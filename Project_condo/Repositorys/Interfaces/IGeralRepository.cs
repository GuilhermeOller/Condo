using Project_Condo.Models;

namespace Project_Condo.Repositorys.Interfaces
{
    public interface IGeralRepository
    {
        Task<bool> VerificarConflitoEmail(string email);
        Task<bool> GravarLogCadastroAsync(LogCadastro logCadastro);
        Task<bool> GravarLogResetPasswordAsync(LogResetPassword logReset);
        Task<bool> VerificaToken(string token, string email);
        Task<Moradores> VerificarMoradorPorEmail(string email);
        Task<Login> BuscarLoginPorEmail(string email);
        Task<Moradores> BuscarMoradorPorId(long id);
        Task<string> BuscarNomeMoradorPorEmail(string email);
        Task<Apartamento> BuscarCondominio();
        Task<EmailConfig> BuscarConfigEmail();
        Task<ZapiConfig> BuscarConfigZapi();
        Task UpdateSenha(string novaSenha, string email);
    }
}
