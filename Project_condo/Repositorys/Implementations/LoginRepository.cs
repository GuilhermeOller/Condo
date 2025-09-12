using Project_Condo.Models;
using Project_Condo.Repositorys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Project_Condo.Repositorys.Implementations
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
            return await _context.tblLogin.FirstOrDefaultAsync(u => u.usuario == input || u.email == input);
        }

        public async Task AtualizarUsuario(Login usuario)
        {
            _context.tblLogin.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarLogAsync(LogLogin log)
        {
            _context.tblLogLogin.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarUsuario(Login login)
        {
            _context.tblLogin.Add(login);
            await _context.SaveChangesAsync();
        }
    }
}
