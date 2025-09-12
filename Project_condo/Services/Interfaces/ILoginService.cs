using Project_Condo.Models;

namespace Project_Condo.Services.Interfaces
{
    public interface ILoginService
    {
        Task<(bool Sucesso, string Mensagem)> CadastrarAsync(Login login, HttpContext httpContext);
        Task<(bool Sucesso, string Mensagem)> AutenticarAsync(LoginModel login, HttpContext httpContext);
        Task<(bool Sucesso, string Mensagem)> EnviarTokenAsync(string email, HttpContext httpContext, string ip);
        Task<(bool Sucesso, string Mensagem)> VerificaTokenAsync(string token, HttpContext httpContext);
        Task<(bool Sucesso, string Mensagem)> UpdateSenhaAsync(string novaSenha, HttpContext httpContext);
    }
}
