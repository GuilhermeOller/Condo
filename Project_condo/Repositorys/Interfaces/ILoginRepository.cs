using Project_Condo.Models;

namespace Project_Condo.Repositorys.Interfaces
{
    public interface ILoginRepository
    {
        Task<Login> BuscarUsuario(string input);
        Task AtualizarUsuario(Login usuario);
        Task AdicionarLogAsync(LogLogin log);
        Task AdicionarUsuario(Login login);
    }
}
