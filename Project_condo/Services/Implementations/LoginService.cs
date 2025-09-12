using Project_Condo.Models;
using Project_Condo.Repositorys.Interfaces;
using Project_Condo.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Project_Condo.Services.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IGeralRepository _geralR;
        private readonly ILoginRepository _loginR;
        private readonly IVisitanteRepository _visitanteR;
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

            var usuario = await _loginR.BuscarUsuario(login.usuario);
            if (usuario == null || string.IsNullOrEmpty(usuario.usuario))
                return await FalhaLoginAsync(null, login.ip, "Usuário ou senha inválidos.");

            var morador = await _geralR.BuscarMoradorPorId(usuario.idMorador);
            if (morador.statusMorador == "I")
                return await FalhaLoginAsync(usuario, login.ip, "Usuário cadastrado como Inativo.");

            if (usuario.tentativasLogin >= 5)
                return await FalhaLoginAsync(usuario, login.ip,
                    "Conta bloqueada devido a muitas tentativas falhas, entre em contato com a portaria.");

            if (BCrypt.Net.BCrypt.Verify(login.senha, usuario.senha))
                return await SucessoLoginAsync(usuario, login.ip, httpContext, morador.nomeMorador);

            usuario.tentativasLogin += 1;
            await RegistrarLogLoginAsync(usuario, login.ip, logou: 0);
            await _loginR.AtualizarUsuario(usuario);

            await Task.Delay(2000);
            return (false, "Usuário ou senha inválidos.");
        }

        private bool ValidarLogin(LoginModel login, out string mensagemErro)
        {
            mensagemErro = null;
            if (login == null || string.IsNullOrEmpty(login.usuario) || string.IsNullOrEmpty(login.senha))
                mensagemErro = "Usuário e senha são obrigatórios.";
            else if (string.IsNullOrEmpty(login.ip))
                mensagemErro = "Erro ao processar login.";

            return mensagemErro == null;
        }

        private async Task<(bool, string)> SucessoLoginAsync(Login usuario, string ip, HttpContext httpContext, string nomeMorador)
        {
            usuario.tentativasLogin = 0;
            usuario.dtUltimoLogin = DateTime.Now;

            await _loginR.AtualizarUsuario(usuario);
            await RegistrarLogLoginAsync(usuario, ip, logou: 1);
            await GravarAuthCookieAsync(usuario.idMorador, nomeMorador, usuario.email, httpContext);

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
                ip = ip,
                idMorador = usuario.idMorador,
                data = DateTime.Now,
                flgLogou = logou
            };

            await _loginR.AdicionarLogAsync(log);
        }

        private async Task GravarAuthCookieAsync(long idUsuario, string nome, string email, HttpContext httpContext)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString()),
                new Claim(ClaimTypes.Name, nome),
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
            var morador = await _geralR.VerificarMoradorPorEmail(login.email);

            if (morador == null)
                return (false, "Email não cadastrado no sistema! Entrar em contato com condomínio");

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(login.senha);
            var geral = new Geral(_config, _context, null, _httpClient, _geralR);

            bool conflito = await _geralR.VerificarConflitoEmail(login.email);
            if (conflito)
                return (false, "Email já cadastrado!");

            var novoLogin = new Login
            {
                idMorador = morador.IdMorador,
                usuario = login.usuario,
                senha = senhaHash,
                dtCriacao = DateTime.Now,
                email = morador.emailMorador
            };

            await _loginR.AdicionarUsuario(novoLogin);

            await geral.GravarLogCadastroAsync(novoLogin.idMorador, login.ip);

            return (true, "Usuário cadastrado com sucesso!");
        }
        public async Task<(bool Sucesso, string Mensagem)> EnviarTokenAsync(string email, HttpContext httpContext, string ip)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "E-mail inválido");

            string nomeMorador = await _geralR.BuscarNomeMoradorPorEmail(email);

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
                login.tokenResetSenha = token;
                login.dtExpiracaoToken = expiraToken;
            }
            await _loginR.AtualizarUsuario(login);

            await geral.EnviarEmailComImagemAsync(email, null, nomeMorador, null, null, null, null, null, "Token", token);

            Geral.GravarEmailTemporario(email, httpContext);

            await geral.GravarLogResetPasswordAsync(login.usuario, email, ip);

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
