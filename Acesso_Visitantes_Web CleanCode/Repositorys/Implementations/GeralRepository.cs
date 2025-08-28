using Acesso_Moradores_Visitantes.Models;
using Acesso_Moradores_Visitantes.Repositorys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Acesso_Moradores_Visitantes.Repositorys.Implementations
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
            return await _context.tblLoginMorador.AnyAsync(v => v.email == email);
        }

        public async Task<bool> VerificarCodCondominio(string cod)
        {
            return await _context.tblEmpresa.AnyAsync(v => v.cod == cod);
        }

        public async Task<bool> GravarLogCadastroAsync(LogCadastro logCadastro)
        {
            await _context.tblLogWebCadastro.AddAsync(logCadastro);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> GravarLogResetPasswordAsync(LogResetPassword logReset)
        {
            await _context.tblLogWebResetPassword.AddAsync(logReset);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> VerificaToken(string token, string email)
        {
            return await _context.tblLoginMorador.AnyAsync(v => v.tokenResetSenha == token & v.email == email);
        }

        public async Task<Moradores> VerificarMorador(string email)
        {
            return await _context.tblMorador.FirstOrDefaultAsync(v => v.email == email);
        }

        public async Task<Autorizados> VerificarAutorizado(string email)
        {
            return await _context.tblAutorizados.FirstOrDefaultAsync(v => v.email == email);
        }

        public async Task<Login> BuscarLoginPorEmail(string email)
        {
            return await _context.tblLoginMorador.FirstOrDefaultAsync(v => v.email == email);
        }

        public async Task UpdateSenha(string novaSenha, string email)
        {
            Login login = await _context.tblLoginMorador.FirstOrDefaultAsync(v => v.email == email);

            if (login != null)
            {
                login.senha = novaSenha;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Apartamento> BuscarCondominio()
        {
            return await _context.tblEmpresa.FirstOrDefaultAsync();
        }

        public async Task<EmailConfig> BuscarConfigEmail()
        {
            return await _context.tblEmail.FirstOrDefaultAsync();
        }
        public async Task<ZapiConfig> BuscarConfigZapi()
        {
            return await _context.tblZAPI.FirstOrDefaultAsync();
        }

    }
}
