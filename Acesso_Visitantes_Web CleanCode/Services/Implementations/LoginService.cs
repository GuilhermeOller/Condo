using Acesso_Moradores_Visitantes.Models;
using Acesso_Moradores_Visitantes.Repositorys.Interfaces;
using Acesso_Moradores_Visitantes.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Acesso_Moradores_Visitantes.Services.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IGeralRepository _geralR;
        private readonly ILoginRepository _loginR;
        private readonly ILogger<Geral> _logger;
        private readonly HttpClient _httpClient;

        public LoginService(AppDbContext context, IConfiguration config, ILogger<Geral> logger, HttpClient httpClient, IGeralRepository geralR, ILoginRepository loginR)
        {
            _context = context;
            _config = config;
            _logger = logger;
            _httpClient = httpClient;
            _geralR = geralR;
            _loginR = loginR;
        }

        public async Task<(bool Sucesso, string Mensagem)> AutenticarAsync(LoginModel login, HttpContext httpContext)
        {
            if (!ValidarLogin(login, out string mensagemErro))
                return (false, mensagemErro);

            var usuario = await _loginR.BuscarUsuario(login.Usuario);
            if (usuario == null || string.IsNullOrEmpty(usuario.Usuario))
                return await FalhaLoginAsync(null, login.ip, "Usuário ou senha inválidos.");

            if (usuario.Status == "I")
                return await FalhaLoginAsync(usuario, login.ip, "Usuário cadastrado como Inativo.");

            if (usuario.TentativasLogin >= 5)
                return await FalhaLoginAsync(usuario, login.ip,
                    "Conta bloqueada devido a muitas tentativas falhas, entre em contato com a portaria.");

            if (BCrypt.Net.BCrypt.Verify(login.Senha, usuario.Senha))
                return await SucessoLoginAsync(usuario, login.ip, httpContext);

            usuario.TentativasLogin += 1;
            await RegistrarLogLoginAsync(usuario, login.ip, logou: 0);
            await _loginR.AtualizarUsuario(usuario);

            await Task.Delay(2000);
            return (false, "Usuário ou senha inválidos.");
        }

        private bool ValidarLogin(LoginModel login, out string mensagemErro)
        {
            mensagemErro = null;
            if (login == null || string.IsNullOrEmpty(login.Usuario) || string.IsNullOrEmpty(login.Senha))
                mensagemErro = "Usuário e senha são obrigatórios.";
            else if (string.IsNullOrEmpty(login.ip))
                mensagemErro = "Erro ao processar login.";

            return mensagemErro == null;
        }

        private async Task<(bool, string)> SucessoLoginAsync(Login usuario, string ip, HttpContext httpContext)
        {
            usuario.TentativasLogin = 0;
            usuario.UltimoLogin = DateTime.Now;

            await _loginR.AtualizarUsuario(usuario);
            await RegistrarLogLoginAsync(usuario, ip, logou: 1);
            await GravarAuthCookieAsync(usuario.Id, usuario.NomeMorador, usuario.Tipo, usuario.Email, httpContext);

            return (true, null);
        }

        private async Task<(bool, string)> FalhaLoginAsync(Login usuario, string ip, string mensagem)
        {
            if (usuario != null)
                await RegistrarLogLoginAsync(usuario, ip, logou: 0);

            await Task.Delay(2000);
            return (false, mensagem);
        }

        private async Task RegistrarLogLoginAsync(Login usuario, string ip, short logou)
        {
            if (usuario == null) return;

            var log = new LogLogin
            {
                Ip = ip,
                Nome = usuario.NomeMorador,
                Data = DateTime.Now,
                Logou = logou
            };

            await _loginR.AdicionarLogAsync(log);
        }

        private async Task GravarAuthCookieAsync(long idUsuario, string nome, string tipo, string email, HttpContext httpContext)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString()),
                new Claim(ClaimTypes.Name, nome),
                new Claim("app:TipoLogin", tipo),
                new Claim(ClaimTypes.Email, email),

            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2),
                IsPersistent = true,
                AllowRefresh = true
            };


            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public async Task<(bool Sucesso, string Mensagem)> CadastrarAsync(Login login, HttpContext httpContext)
        {
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(login.Senha);
            var geral = new Geral(_config, _context, null, _httpClient, _geralR);

            bool codOk = await _geralR.VerificarCodCondominio(login.cod);
            if (!codOk)
                return (false, "Código de condomínio incorreto!");

            bool conflito = await _geralR.VerificarConflitoEmail(login.Email);
            if (conflito)
                return (false, "Email já cadastrado!");

            var morador = await _geralR.VerificarMorador(login.Email);
            var autorizado = await _geralR.VerificarAutorizado(login.Email);

            if (morador == null && autorizado == null)
                return (false, "Email não cadastrado no sistema! Entrar em contato com condomínio");

            var novoLogin = new Login
            {
                Id = morador == null ? autorizado.IdAutorizado : morador.IdMorador,
                Usuario = login.Usuario,
                Senha = senhaHash,
                Status = morador == null ? autorizado.flgExcluido == 0 ? "a" : "i" : morador.flgStatus,
                CriadoEm = DateTime.Now,
                NomeMorador = morador == null ? autorizado.Nome : morador.Morador,
                Tipo = morador == null ? "autorizado" : "morador",
                Email = morador == null ? autorizado.email : morador.email
            };
            if (await geral.CadastrarLoginFireBaseAsync(novoLogin.Usuario, novoLogin.Email, login.cod) )
            await _loginR.AdicionarUsuario(novoLogin);

            await geral.GravarLogCadastroAsync(novoLogin.Usuario, login.ip);

            return (true, "Usuário cadastrado com sucesso!");
        }
        public async Task<(bool Sucesso, string Mensagem)> EnviarTokenAsync(string email, HttpContext httpContext, string ip)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "E-mail inválido");

            var geral = new Geral(_config, _context, _logger, _httpClient, _geralR);
            DateTime expiraToken = DateTime.Now.AddMinutes(30);
            string token = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();

            bool conflito = await _geralR.VerificarConflitoEmail(email);

            if (conflito == false)
            {
                return (false, "Email não consta no banco de dados!");
            }

            Login login = await _geralR.BuscarLoginPorEmail(email);

            if (login != null)
            {
                login.TokenResetSenha = token;
                login.TokenExpiraEm = expiraToken;
            }
            await _loginR.AtualizarUsuario(login);

            await geral.EnviarEmailComImagemAsync(email, null, login.NomeMorador, null, null, null, null, null, "Token", token);

            Geral.GravarEmailTemporario(email, httpContext);

            await geral.GravarLogResetPasswordAsync(login.Usuario, email, ip);

            return (true, "Token enviado para email!");
        }

        public async Task<(bool Sucesso, string Mensagem)> VerificaTokenAsync(string token, HttpContext httpContext)
        {
            var email = httpContext.Request.Cookies["email"];
            if (string.IsNullOrEmpty(email))
            {
                return (false, "Por favor, comece o procedimento novamente!");
            }

            bool tokenValido = await _geralR.VerificaToken(token, email);

            if (tokenValido == true)
            {
                return (true, "Cadastro realizado com sucesso!");
            }
            else
            {
                return (false, "Insira corretamente o token!");
            }
        }
        public async Task<(bool Sucesso, string Mensagem)> UpdateSenhaAsync(string novaSenha, HttpContext httpContext)
        {
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);

            var email = httpContext.Request.Cookies["email"];
            if (string.IsNullOrEmpty(email))
            {
                return (false, "Por favor, comece o procedimento novamente!");
            }

            await _geralR.UpdateSenha(senhaHash, email);

            return (true, "Senha alterada com sucesso!");
        }
    }

}
