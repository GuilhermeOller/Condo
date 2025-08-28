using Acesso_Moradores_Visitantes.Models;
using Acesso_Moradores_Visitantes.Repositorys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Acesso_Moradores_Visitantes.Repositorys.Implementations
{
    public class LoginRepository : ILoginRepository
    {
        private readonly AppDbContext _context;

        public LoginRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Login> BuscarUsuario(string input)
        {
            return await _context.tblLoginMorador.FirstOrDefaultAsync(u => u.usuario == input || u.email == input);
        }

        public async Task AtualizarUsuario(Login usuario)
        {
            _context.tblLoginMorador.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarLogAsync(LogLogin log)
        {
            _context.tblLogWebLogin.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarUsuario(Login login)
        {
            _context.tblLoginMorador.Add(login);
            await _context.SaveChangesAsync();
        }
    }
}
