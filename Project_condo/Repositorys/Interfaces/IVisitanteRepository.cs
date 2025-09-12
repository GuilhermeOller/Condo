using Project_Condo.Models;

namespace Project_Condo.Repositorys.Interfaces
{
    public interface IVisitanteRepository
    {
        //Visitantes
        Task<List<Visitantes>> ObterVisitantesAsync(long idUsuario);
        Task<Visitantes> BuscarVisitantePorNomeAsync(string nomeVisitante);
        Task<Visitantes> BuscarVisitantePorIdAsync(long idVisitante);
        Task AdicionarVisitanteAsync(Visitantes visitante);
        Task AtualizarVisitanteAsync(Visitantes visitante);
        Task RemoverRelacaoUsuarioVisitanteAsync(MorVis visitante);
        Task<MorVis> VerificaRealacaoUsuarioVisitante(long idUsuario, long idVisitante);
        Task AdicionarRelacaoUsuarioVisitanteAsync(MorVis novaRelacao);
        //Festas
        Task<List<Festas>> ObterFestasPorUsuarioAsync(long idUsuario);
        Task<Festas> BuscarFestaPorIdAsync(long idFesta);
        Task AdicionarNovaFestaAsync(Festas novaFesta);
        Task AtualizarFestaAsync(Festas festa);
        Task DeletarFestaAsync(Festas festa);

        Task<List<FestaVisitante>> ObterVisitantesPorFestaAsync(long idFesta);
        Task<FestaVisitante> BuscarRelacaoFestaVisitanteAsync(long idVisitante, long idFesta);
        Task AdicionarRelacaoFestaVisitanteAsync(FestaVisitante novaFesta);
        Task RemoverRelacaoFestaVisitanteAsync(FestaVisitante festa);
        //Acessos
        Task<bool> VerificaAcessoEmAbertoAsync(long idVisitante, long idUsuario);
        Task<long> BuscarIdAcessoPorIdAsync(long idVisitante, long idUsuario);
        Task<Acessos> BuscarAcessoPorIdAsync(long idAcesso);
        Task AdicionarNovoAcessoAsync(Acessos novoAcesso);

        Task AtualizarAcessoAsync(Acessos acesso);
        //Logs
        Task AdicionarLogAcessoAsync(LogAcessos novoLog);
        //Outros
        Task<string> BuscarPathImagesAsync();
    }
}
