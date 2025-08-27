using Acesso_Moradores_Visitantes.Models;
using Acesso_Moradores_Visitantes.Repositorys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Acesso_Moradores_Visitantes.Repositorys.Implementations
{
    public class VisitanteRepository : IVisitanteRepository
    {
        private readonly AppDbContext _context;

        public VisitanteRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Visitantes
        public async Task<List<Visitantes>> ObterVisitantesAsync(long idUsuario, string tipoLogin)
        {
            return await _context.tblUsuarioVisitante
                .Where(uv => uv.IdUsuario == idUsuario && uv.Tipo == tipoLogin)
                .Select(uv => uv.Visitante)
                .ToListAsync();
        }

        public async Task<Visitantes> BuscarVisitantePorNomeAsync(string nomeVisitante)
        {
            return await _context.tblVisitante.FirstOrDefaultAsync(v => v.Visitante == nomeVisitante);
        }

        public async Task<Visitantes> BuscarVisitantePorIdAsync(long idVisitante)
        {
            return await _context.tblVisitante.FirstOrDefaultAsync(v => v.idVisitante == idVisitante);
        }

        public async Task AdicionarVisitanteAsync(Visitantes visitante)
        {
            _context.tblVisitante.Add(visitante);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarVisitanteAsync(Visitantes visitante)
        {
            _context.tblVisitante.Update(visitante);
            await _context.SaveChangesAsync();
        }

        public async Task<UsuarioVisitante> VerificaRealacaoUsuarioVisitante(long idUsuario, long idVisitante, string tipoLogin)
        {
            return await _context.tblUsuarioVisitante.FirstOrDefaultAsync(uv =>
                                uv.IdUsuario == idUsuario &&
                                uv.IdVisitante == idVisitante &&
                                uv.Tipo == tipoLogin);
        }

        public async Task AdicionarRelacaoUsuarioVisitanteAsync(UsuarioVisitante novaRelacao)
        {
            _context.tblUsuarioVisitante.Add(novaRelacao);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverRelacaoUsuarioVisitanteAsync(UsuarioVisitante visitante)
        {
            _context.tblUsuarioVisitante.Remove(visitante);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Festas
        public async Task<List<Festas>> ObterFestasPorUsuarioAsync(long idUsuario, string tipoLogin)
        {
            return await _context.tblFestas
                .Where(v => v.idUsuario == idUsuario &&
                            v.tipoUsuario == tipoLogin &&
                            v.ate > DateTime.Now)
                .Select(v => new Festas
                {
                    id = v.id,
                    nomeFesta = v.nomeFesta,
                    de = v.de,
                    ate = v.ate
                })
                .ToListAsync();
        }

        public async Task<Festas> BuscarFestaPorIdAsync(long idFesta)
        {
            return await _context.tblFestas.FirstOrDefaultAsync(a => a.id == idFesta);
        }

        public async Task AdicionarNovaFestaAsync(Festas novaFesta)
        {
            _context.tblFestas.Add(novaFesta);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarFestaAsync(Festas festa)
        {
            _context.tblFestas.Update(festa);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarFestaAsync(Festas festa)
        {
            _context.tblFestas.Remove(festa);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FestaVisitante>> ObterVisitantesPorFestaAsync(long idFesta)
        {
            return await _context.tblFestaVisitante
                     .Where(uv => uv.idFesta == idFesta)
                     .ToListAsync();
        }

        public async Task<FestaVisitante> BuscarRelacaoFestaVisitanteAsync(long idVisitante, long idFesta)
        {
            return await _context.tblFestaVisitante
                         .FirstOrDefaultAsync(v => v.idVisitante == idVisitante && v.idFesta == idFesta);
        }

        public async Task AdicionarRelacaoFestaVisitanteAsync(FestaVisitante novaFesta)
        {
            _context.tblFestaVisitante.Add(novaFesta);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverRelacaoFestaVisitanteAsync(FestaVisitante festa)
        {
            _context.tblFestaVisitante.Remove(festa);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Acessos
        public async Task<bool> VerificaAcessoEmAbertoAsync(long idVisitante, long idUsuario, string tipoLogin)
        {
            return await _context.tblAcessos
                    .AnyAsync(va =>
                        va.IdOrigem == idVisitante &&
                        va.FlgOrigem == "V" &&
                        va.IdVisitado == idUsuario &&
                        va.TipoVisitado == tipoLogin &&
                        va.DtSaida == null);
        }

        public async Task<long> BuscarIdAcessoPorIdAsync(long idVisitante, long idUsuario)
        {
            return await _context.tblAcessos
                         .Where(v => v.IdOrigem == idVisitante && v.IdVisitado == idUsuario && v.DtSaida == null)
                         .Select(v => v.IdAcesso)
                         .FirstOrDefaultAsync();
        }

        public async Task<Acessos> BuscarAcessoPorIdAsync(long idAcesso)
        {
            return await _context.tblAcessos.FirstOrDefaultAsync(a => a.IdAcesso == idAcesso);
        }

        public async Task<VisAcesso> BuscarVisAcessoPorIdAsync(long idAcesso)
        {
            return await _context.tblVisAcesso.FirstOrDefaultAsync(a => a.IdAcesso == idAcesso);
        }

        public async Task AdicionarNovoAcessoAsync(Acessos novoAcesso)
        {
            _context.tblAcessos.Add(novoAcesso);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarNovoVisAcessoAsync(VisAcesso novoVisAcesso)
        {
            _context.tblVisAcesso.Add(novoVisAcesso);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarNovoMorAcessoAsync(MorAcesso novoMorAcesso)
        {
            _context.tblMorAcesso.Add(novoMorAcesso);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAcessoAsync(Acessos acesso)
        {
            _context.tblAcessos.Update(acesso);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarVisAcessoAsync(VisAcesso visAcesso)
        {
            _context.tblVisAcesso.Update(visAcesso);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Logs
        public async Task AdicionarLogAcessoAsync(LogAcessos novoLog)
        {
            _context.tblLogWebAcessos.Add(novoLog);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Outros / Utilitários
        public async Task<Autorizados> BuscarAutorizadoPorIdAsync(long idAutorizado)
        {
            return await _context.tblAutorizados.FirstOrDefaultAsync(a => a.IdAutorizado == idAutorizado);
        }

        public async Task<string> BuscarPathImagesAsync()
        {
            return await _context.tblParametros.Select(p => p.PathImages).FirstOrDefaultAsync();
        }
        #endregion
    }
}
