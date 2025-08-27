using Acesso_Moradores_Visitantes.Models;

namespace Acesso_Moradores_Visitantes.Repositorys.Interfaces
{
    public interface IGeralRepository
    {
        Task<bool> VerificarConflitoEmail(string email);
        Task<bool> VerificarCodCondominio(string cod);
        Task<bool> GravarLogCadastroAsync(LogCadastro logCadastro);
        Task<bool> GravarLogResetPasswordAsync(LogResetPassword logReset);
        Task<bool> VerificaToken(string token, string email);
        Task<Moradores> VerificarMorador(string email);
        Task<Autorizados> VerificarAutorizado(string email);
        Task<Login> BuscarLoginPorEmail(string email);
        Task<Condominio> BuscarCondominio();
        Task<Email> BuscarConfigEmail();
        Task<Zapi> BuscarConfigZapi();
        Task UpdateSenha(string novaSenha, string email);
    }
}
