using Acesso_Moradores_Visitantes.Models;

namespace Acesso_Moradores_Visitantes.Repositorys.Interfaces
{
    public interface ILoginRepository
    {
        Task<Login> BuscarUsuario(string input);
        Task AtualizarUsuario(Login usuario);
        Task AdicionarLogAsync(LogLogin log);
        Task AdicionarUsuario(Login login);
    }
}
