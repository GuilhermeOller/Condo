using Project_Condo.Models;
using Project_Condo.Repositorys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Project_Condo.Repositorys.Implementations
{
    public class VisitanteRepository : IVisitanteRepository
    {
        private readonly AppDbContext _context;

        public VisitanteRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Visitantes
        public async Task<List<Visitantes>> ObterVisitantesAsync(long idUsuario)
        {
            return await _context.tblMorVis
                .Where(uv => uv.idMorador == idUsuario)
                .Select(uv => uv.Visitante)
                .ToListAsync();
        }

        public async Task<Visitantes> BuscarVisitantePorNomeAsync(string nomeVisitante)
        {
            return await _context.tblVisitantes.FirstOrDefaultAsync(v => v.nomeVisitante == nomeVisitante);
        }

        public async Task<Visitantes> BuscarVisitantePorIdAsync(long idVisitante)
        {
            return await _context.tblVisitantes.FirstOrDefaultAsync(v => v.idVisitante == idVisitante);
        }

        public async Task AdicionarVisitanteAsync(Visitantes visitante)
        {
            _context.tblVisitantes.Add(visitante);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarVisitanteAsync(Visitantes visitante)
        {
            _context.tblVisitantes.Update(visitante);
            await _context.SaveChangesAsync();
        }

        public async Task<MorVis> VerificaRealacaoUsuarioVisitante(long idUsuario, long idVisitante)
        {
            return await _context.tblMorVis.FirstOrDefaultAsync(uv =>
                                uv.idMorador == idUsuario &&
                                uv.idVisitante == idVisitante);
        }

        public async Task AdicionarRelacaoUsuarioVisitanteAsync(MorVis novaRelacao)
        {
            _context.tblMorVis.Add(novaRelacao);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverRelacaoUsuarioVisitanteAsync(MorVis visitante)
        {
            _context.tblMorVis.Remove(visitante);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Festas
        public async Task<List<Festas>> ObterFestasPorUsuarioAsync(long idUsuario)
        {
            return await _context.tblFestas
                .Where(v => v.idUsuario == idUsuario &&
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
        public async Task<bool> VerificaAcessoEmAbertoAsync(long idVisitante, long idUsuario)
        {
            return await _context.tblAcessos
                    .AnyAsync(va =>
                        va.idVisitante == idVisitante &&
                        va.idMorador == idUsuario &&
                        va.saida == null);
        }

        public async Task<long> BuscarIdAcessoPorIdAsync(long idVisitante, long idUsuario)
        {
            return await _context.tblAcessos
                         .Where(v => v.idVisitante == idVisitante && v.idMorador == idUsuario && v.saida == null)
                         .Select(v => v.idAcesso)
                         .FirstOrDefaultAsync();
        }

        public async Task<Acessos> BuscarAcessoPorIdAsync(long idAcesso)
        {
            return await _context.tblAcessos.FirstOrDefaultAsync(a => a.idAcesso == idAcesso);
        }

        public async Task AdicionarNovoAcessoAsync(Acessos novoAcesso)
        {
            _context.tblAcessos.Add(novoAcesso);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAcessoAsync(Acessos acesso)
        {
            _context.tblAcessos.Update(acesso);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Logs
        public async Task AdicionarLogAcessoAsync(LogAcessos novoLog)
        {
            _context.tblLogAcessos.Add(novoLog);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Outros / Utilitários
        public async Task<string> BuscarPathImagesAsync()
        {
            return await _context.tblParametros.Select(p => p.pathImagens).FirstOrDefaultAsync();
        }
        #endregion
    }
}
