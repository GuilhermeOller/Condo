using Microsoft.EntityFrameworkCore;
using Project_Condo.Models;
using Project_Condo.Repositorys.Interfaces;
using static QRCoder.Core.PayloadGenerator;

namespace Project_Condo.Repositorys.Implementations
{
    public class GeralRepository : IGeralRepository
    {
        private readonly AppDbContext _context;

        public GeralRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> VerificarConflitoEmail(string email)
        {
            return await _context.tblLogin.AnyAsync(v => v.email == email);
        }

        public async Task<bool> GravarLogCadastroAsync(LogCadastro logCadastro)
        {
            await _context.tblLogCadastroLogin.AddAsync(logCadastro);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> GravarLogResetPasswordAsync(LogResetPassword logReset)
        {
            await _context.tblLogResetPasswordLogin.AddAsync(logReset);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> VerificaToken(string token, string email)
        {
            return await _context.tblLogin.AnyAsync(v => v.tokenResetSenha == token & v.email == email);
        }

        public async Task<Moradores> VerificarMoradorPorEmail(string email)
        {
            return await _context.tblMoradores.FirstOrDefaultAsync(v => v.emailMorador == email);
        }

        public async Task<Login> BuscarLoginPorEmail(string email)
        {
            return await _context.tblLogin.FirstOrDefaultAsync(v => v.email == email);
        }

        public async Task<Moradores> BuscarMoradorPorId(long id)
        {
            return await _context.tblMoradores.FirstOrDefaultAsync(v => v.IdMorador == id);
        }
        public async Task<string> BuscarNomeMoradorPorEmail(string email)
        {
            string nomeMorador = await _context.tblMoradores.Where(m => m.emailMorador == email)
                .Select(m => m.nomeMorador)
                .FirstOrDefaultAsync();

            return nomeMorador;
        }

        public async Task UpdateSenha(string novaSenha, string email)
        {
            Login login = await _context.tblLogin.FirstOrDefaultAsync(v => v.email == email);

            if (login != null)
            {
                login.senha = novaSenha;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Apartamento> BuscarCondominio()
        {
            return await _context.tblApartamento.FirstOrDefaultAsync();
        }

        public async Task<EmailConfig> BuscarConfigEmail()
        {
            return await _context.tblEmailConfig.FirstOrDefaultAsync();
        }
        public async Task<ZapiConfig> BuscarConfigZapi()
        {
            return await _context.tblZapiConfig.FirstOrDefaultAsync();
        }

    }
}
