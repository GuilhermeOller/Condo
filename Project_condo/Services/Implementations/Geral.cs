using Project_Condo.Models;
using Project_Condo.Repositorys.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using QRCoder;
using QRCoder.Core;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Project_Condo.Services.Implementations
{
    public class Geral
    {
        private readonly IConfiguration _config;
        private readonly IGeralRepository _geralR;
        private readonly AppDbContext _context;
        private readonly ILogger<Geral> _logger;
        private readonly HttpClient _httpClient;

        public Geral(IConfiguration config, AppDbContext context, ILogger<Geral> logger, HttpClient httpClient, IGeralRepository geralR)
        {
            _config = config;
            _context = context;
            _logger = logger;
            _httpClient = httpClient;
            _geralR = geralR;
        }

        public async Task<string> GetPublicIpAsync(HttpContext httpContext)
        {
            string ipUsuario = httpContext.Connection.RemoteIpAddress?.ToString();
            return ipUsuario;
        }

        //public static async Task<string> GetPublicIpAsync()
        //{
        //    var host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (var ip in host.AddressList)
        //    {
        //        if (ip.AddressFamily == AddressFamily.InterNetwork)
        //        {
        //            return ip.ToString();
        //        }
        //    }

        //    return "IP local não encontrado.";
        //}

        public static void GravarEmailTemporario(string email, HttpContext httpContext)
        {
            httpContext.Response.Cookies.Append("email", email, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(15),
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });
        }

        public async Task EnviarEmailComImagemAsync(string email, string nomeFesta, string nomeUsuario,
            string imagemBase64, string nomeVisitante, string numCracha, string de, string ate, string tipo, string token)
        {
            string smtpServer = "";
            int smtpPort = 0;
            string smtpUser = "";
            string smtpPass = "";
            string numeroCondominio = string.Empty;
            string nomeCondominio = string.Empty;
            string linkWhatsapp = string.Empty; // preencha ou remova
            var msg = $"";

            try
            {
                var empresa = await _geralR.BuscarCondominio();
                if (empresa != null)
                {
                    nomeCondominio = empresa.nomeApartamento;
                    numeroCondominio = empresa.telefoneApartamento;
                }

                var emailConfig = await _geralR.BuscarConfigEmail();
                if (emailConfig != null)
                {
                    smtpServer = emailConfig.smtpServer;
                    smtpPort = int.TryParse(emailConfig.smtpPort, out int port) ? port : 0;
                    smtpUser = emailConfig.email;
                    smtpPass = emailConfig.senha;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(nomeCondominio, smtpUser));
                message.To.Add(new MailboxAddress(nomeVisitante, email));

                if (tipo == "Convite")
                {
                    msg = $"Olá {nomeVisitante}, o condomínio {nomeCondominio} avisa que você foi convidado para a festa {nomeFesta} pelo usuário {nomeUsuario}. " +
                          $"Dos dias {de} até {ate}. " +
                          $"Utilize o QR Code em anexo para acessar o condomínio.\n\nQualquer dúvida, acesse:\n{linkWhatsapp}";

                    message.Subject = $"Convite para {nomeFesta}";
                }

                if (tipo == "Token")
                {
                    msg = $"Olá {nomeUsuario}, o condomínio {nomeCondominio} avisa que esse é seu token para mudar sua senha. \n" +
                          $"\n{token}\n" +
                          $"\nO token tem validade de 30 minutos, apresente ele para trocar sua senha.\n\nQualquer dúvida, acesse:\n{linkWhatsapp}";

                    message.Subject = $"Token Nova Senha";
                }

                var builder = new BodyBuilder
                {
                    TextBody = msg
                };

                if (!string.IsNullOrWhiteSpace(imagemBase64))
                {
                    var base64Data = imagemBase64.Contains(",") ? imagemBase64.Split(',')[1] : imagemBase64;
                    var imageBytes = Convert.FromBase64String(base64Data);
                    builder.Attachments.Add("convite.jpg", imageBytes, new ContentType("image", "jpeg"));
                }

                if (!string.IsNullOrEmpty(numCracha))
                {
                    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                    using (QRCodeData qrData = qrGenerator.CreateQrCode(numCracha, QRCodeGenerator.ECCLevel.Q))
                    using (QRCode qrCode = new QRCode(qrData))
                    using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        qrBitmap.Save(ms, ImageFormat.Png);
                        builder.Attachments.Add("QrCodeAcesso.png", ms.ToArray(), new ContentType("image", "png"));
                    }
                }

                message.Body = builder.ToMessageBody();

                if (smtpPort > 0)
                {
                    using var client = new SmtpClient();
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.Auto);
                    await client.AuthenticateAsync(smtpUser, smtpPass);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail com MailKit");
                throw;
            }
        }

        //public async Task GravarCookieEmail(HttpContext httpContext, string email)
        //{
        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.Email, email),
        //    };

        //    var claimsIdentity = new ClaimsIdentity(
        //        claims,
        //        CookieAuthenticationDefaults.AuthenticationScheme);

        //    var authProperties = new AuthenticationProperties
        //    {
        //        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2),
        //        IsPersistent = true,
        //        AllowRefresh = true
        //    };

        //    await httpContext.SignInAsync(
        //        CookieAuthenticationDefaults.AuthenticationScheme,
        //        new ClaimsPrincipal(claimsIdentity),
        //        authProperties);
        //}

        public async Task<bool> GravarLogCadastroAsync(long idMorador, string ip)
        {

            var logCadastro = new LogCadastro()
            {
                idMorador = idMorador,
                data = DateTime.Now,
                ip = ip
            };

            return await _geralR.GravarLogCadastroAsync(logCadastro);
        }
        public async Task<bool> GravarLogResetPasswordAsync(string usuario, string email, string ip)
        {
            var logReset = new LogResetPassword()
            {
                idMorador = usuario,
                email = email,
                data = DateTime.Now,
                ip = ip
            };

            return await _geralR.GravarLogResetPasswordAsync(logReset);
        }
        public async Task EnviarZapViaZAPIAsync(string numeroVisitante, string nomeVisitante, string numCracha,
          string de, string ate, string tipo, string nomeFesta, string nomeUsuario, string convite)
        {
            string numeroCondominio = string.Empty;
            string nomeCondominio = string.Empty;
            string instancia = string.Empty;
            string tokenInstancia = string.Empty;
            string tokenSeguranca = string.Empty;
            string linkWhatsapp = string.Empty;
            string msg = "";

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrData = qrGenerator.CreateQrCode(numCracha, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrData))
            using (Bitmap qrBitmap = qrCode.GetGraphic(20))
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    qrBitmap.Save(ms, ImageFormat.Png);

                    byte[] imageBytes = ms.ToArray();
                    string base64Image = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
                    using (var conn = new SqlConnection(_config.GetConnectionString("dbAcesso")))
                    {
                        await conn.OpenAsync();

                        var cmdEmpresa = await _geralR.BuscarCondominio();
                        nomeCondominio = cmdEmpresa.nomeApartamento;
                        numeroCondominio = cmdEmpresa.telefoneApartamento;

                        var cmdZapi = await _geralR.BuscarConfigZapi();
                    }
                    if (tipo == "festaComConviteUp" || tipo == "festaComConvite")
                    {
                        if (tipo == "festaComConviteUp")
                        {
                            msg =
                                $"Olá! {nomeVisitante}, o condomínio {nomeCondominio} gostaria de avisar que " +
                                $"\nO {nomeUsuario} atualizou a data da festa {nomeFesta}" +
                                $"\nAgora acontecerá dos dias {de} ate {ate}" +
                                $"\nUse o QR Code a seguir para acessar" +
                                $"\nDúvidas? Contate: \n {linkWhatsapp}";
                        }
                        else if (tipo == "festaComConvite")
                        {
                            msg =
                                $"Olá! {nomeVisitante}, o condomínio {nomeCondominio} gostaria de avisar que " +
                                $"\nvocê foi convidado para a festa {nomeFesta} pelo {nomeUsuario}" +
                                $"\nDos dias {de} ate {ate}" +
                                $"\nUse o QR Code a seguir para acessar" +
                                $"\nDúvidas? Contate: \n {linkWhatsapp}";
                        }

                        base64Image = "data:image/png;base64," + convite;
                    }
                    else if (tipo == "apenasQr")
                    {
                        msg = "";
                    }
                    else if (tipo == "festaSemConviteUp" || tipo == "festaSemConvite")
                    {
                        if (tipo == "festaSemConviteUp")
                        {
                            msg =
                                $"Olá! {nomeVisitante}, o condomínio {nomeCondominio} gostaria de avisar que " +
                                $"\nO {nomeUsuario} atualizou a data da festa {nomeFesta}" +
                                $"\nAgora acontecerá dos dias {de} ate {ate}" +
                                $"\nUse o QR Code a seguir para acessar" +
                                $"\nDúvidas? Contate: \n {linkWhatsapp}";
                        }
                        else if (tipo == "festaSemConvite")
                        {
                            msg =
                                $"Olá! {nomeVisitante}, o condomínio {nomeCondominio} gostaria de avisar que " +
                                $"\nvocê foi convidado para a festa {nomeFesta} pelo {nomeUsuario}" +
                                $"\nDos dias {de} ate {ate}" +
                                $"\nUse o QR Code a seguir para acessar" +
                                $"\nDúvidas? Contate: \n {linkWhatsapp}";
                        }
                    }
                    else
                    {
                        msg =
                            $"Olá! {nomeVisitante}, o condomínio {nomeCondominio} gostaria de avisar que " +
                            $"\nseu acesso foi liberado pelo {nomeUsuario}" +
                            $"\nDos dias {de} ate {ate}" +
                            $"\nUse o QR Code a seguir para acessar" +
                            $"\nDúvidas? Contate: \n {linkWhatsapp}";
                    }

                    // 2. Enviar imagem via Z-API
                    var urlImagem = $"https://api.z-api.io/instances/{instancia}/token/{tokenInstancia}/send-image";
                    var payloadImagem = new
                    {
                        phone = "55" + numeroVisitante,
                        image = base64Image,
                        caption = msg,
                        viewOnce = false
                    };

                    using var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("client-token", tokenSeguranca);

                    var jsonImg = JsonConvert.SerializeObject(payloadImagem);
                    var contentImg = new StringContent(jsonImg, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(urlImagem, contentImg);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        _logger.LogError("Erro ao enviar imagem: " + error);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao enviar mensagem via ZAPI");
                }
            }


        }
    }

}
