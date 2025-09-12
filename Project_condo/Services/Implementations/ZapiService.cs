using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using QRCoder.Core;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Project_Condo.Services.Implementations
{
    public class ZapiService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ZapiService> _logger;
        private readonly IWebHostEnvironment _env;

        public ZapiService(IConfiguration config, ILogger<ZapiService> logger, IWebHostEnvironment env)
        {
            _config = config;
            _logger = logger;
            _env = env;
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

                        var cmdEmpresa = new SqlCommand("SELECT TOP 1 Fantazia, fax FROM tblEmpresa", conn);
                        using (var reader = await cmdEmpresa.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                nomeCondominio = reader["Fantazia"].ToString();
                                numeroCondominio = reader["fax"].ToString();
                            }
                        }

                        var cmdZapi = new SqlCommand("SELECT TOP 1 * FROM tblZAPI", conn);
                        using (var reader = await cmdZapi.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                instancia = reader["Instancia"].ToString();
                                tokenInstancia = reader["TokenInstancia"].ToString();
                                tokenSeguranca = reader["TokenSeguranca"].ToString();
                                linkWhatsapp = reader["WaMe"].ToString();
                            }
                        }
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


        //public async Task EnviarZapViaZAPIAsync(string numeroVisitante, string nomeVisitante, string numCracha, string baseUrl)
        //{
        //    string numeroCondominio = string.Empty;
        //    string nomeCondominio = string.Empty;
        //    string instancia = string.Empty;
        //    string tokenInstancia = string.Empty;
        //    string tokenSeguranca = string.Empty;

        //    try
        //    {
        //        using (var conn = new SqlConnection(_config.GetConnectionString("dbAcesso")))
        //        {
        //            await conn.OpenAsync();

        //            var cmdEmpresa = new SqlCommand("SELECT TOP 1 Fantazia, fax FROM tblEmpresa", conn);
        //            using (var reader = await cmdEmpresa.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    nomeCondominio = reader["Fantazia"].ToString();
        //                    numeroCondominio = reader["fax"].ToString();
        //                }
        //            }

        //            var cmdZapi = new SqlCommand("SELECT TOP 1 Instancia, TokenInstancia, TokenSeguranca FROM tblZAPI", conn);
        //            using (var reader = await cmdZapi.ExecuteReaderAsync())
        //            {
        //                if (await reader.ReadAsync())
        //                {
        //                    instancia = reader["Instancia"].ToString();
        //                    tokenInstancia = reader["TokenInstancia"].ToString();
        //                    tokenSeguranca = reader["TokenSeguranca"].ToString();
        //                }
        //            }
        //        }

        //        // 1. Gerar QR Code em base64
        //        string fileName = $"{numCracha}.png";
        //        string qrFolder = Path.Combine(_env.WebRootPath, "qrcodes");
        //        Directory.CreateDirectory(qrFolder); // Garante que a pasta existe
        //        string qrPath = Path.Combine(qrFolder, fileName);

        //        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        //        using (QRCodeData qrData = qrGenerator.CreateQrCode(numCracha, QRCodeGenerator.ECCLevel.Q))
        //        using (QRCode qrCode = new QRCode(qrData))
        //        using (Bitmap qrBitmap = qrCode.GetGraphic(20))
        //        {
        //            qrBitmap.Save(qrPath, ImageFormat.Png);
        //        }
        //        byte[] imageBytes = await File.ReadAllBytesAsync(qrPath);
        //        string base64Image = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
        //        string qrUrl = $"{baseUrl}/qrcodes/{fileName}";
        //        // 2. Enviar mensagem de texto
        //        string msg = $"Olá! {nomeVisitante}, o condomínio {nomeCondominio} liberou sua entrada." +
        //       $"\nUse o QR Code a seguir para acessar" +
        //       $"\nDúvidas? Contate: {numeroCondominio}";

        //        var urlTexto = $"https://api.z-api.io/instances/{instancia}/token/{tokenInstancia}/send-text";
        //        var payloadTexto = new
        //        {
        //            phone = "55" + numeroVisitante,
        //            message = msg
        //        };

        //        using var httpClient = new HttpClient();
        //        httpClient.DefaultRequestHeaders.Add("client-token", tokenSeguranca);

        //        var jsonTexto = JsonConvert.SerializeObject(payloadTexto);
        //        var contentTexto = new StringContent(jsonTexto, Encoding.UTF8, "application/json");
        //        await httpClient.PostAsync(urlTexto, contentTexto);

        //        // 3. Enviar QR Code como image

        //        var urlImagem = $"https://api.z-api.io/instances/{instancia}/token/{tokenInstancia}/send-file-image";
        //        var payloadImagem = new
        //        {
        //            phone = "55" + numeroVisitante,
        //            Image = base64Image,
        //            caption = $"QR Code de acesso - {numCracha}",
        //            viewOnce = false
        //        };

        //        var jsonImg = JsonConvert.SerializeObject(payloadImagem);
        //        var contentImg = new StringContent(jsonImg, Encoding.UTF8, "application/json");

        //        httpClient.DefaultRequestHeaders.Remove("client-token");
        //        httpClient.DefaultRequestHeaders.Add("client-token", tokenSeguranca);

        //        await httpClient.PostAsync(urlImagem, contentImg);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro ao enviar QR Code via ZAPI");
        //    }
        //}

    }
}