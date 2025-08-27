using Acesso_Moradores_Visitantes.Models;

namespace Acesso_Moradores_Visitantes.Repositorys.Interfaces
{
    public interface IVisitanteRepository
    {
        //Visitantes
        Task<List<Visitantes>> ObterVisitantesAsync(long idUsuario, string tipoLogin);
        Task<Visitantes> BuscarVisitantePorNomeAsync(string nomeVisitante);
        Task<Visitantes> BuscarVisitantePorIdAsync(long idVisitante);
        Task AdicionarVisitanteAsync(Visitantes visitante);
        Task AtualizarVisitanteAsync(Visitantes visitante);
        Task RemoverRelacaoUsuarioVisitanteAsync(UsuarioVisitante visitante);
        Task<UsuarioVisitante> VerificaRealacaoUsuarioVisitante(long idUsuario, long idVisitante, string tipoLogin);
        Task AdicionarRelacaoUsuarioVisitanteAsync(UsuarioVisitante novaRelacao);
        //Festas
        Task<List<Festas>> ObterFestasPorUsuarioAsync(long idUsuario, string tipoLogin);
        Task<Festas> BuscarFestaPorIdAsync(long idFesta);
        Task AdicionarNovaFestaAsync(Festas novaFesta);
        Task AtualizarFestaAsync(Festas festa);
        Task DeletarFestaAsync(Festas festa);

        Task<List<FestaVisitante>> ObterVisitantesPorFestaAsync(long idFesta);
        Task<FestaVisitante> BuscarRelacaoFestaVisitanteAsync(long idVisitante, long idFesta);
        Task AdicionarRelacaoFestaVisitanteAsync(FestaVisitante novaFesta);
        Task RemoverRelacaoFestaVisitanteAsync(FestaVisitante festa);
        //Acessos
        Task<bool> VerificaAcessoEmAbertoAsync(long idVisitante, long idUsuario, string tipoLogin);
        Task<long> BuscarIdAcessoPorIdAsync(long idVisitante, long idUsuario);
        Task<Acessos> BuscarAcessoPorIdAsync(long idAcesso);
        Task<VisAcesso> BuscarVisAcessoPorIdAsync(long idAcesso);

        Task AdicionarNovoAcessoAsync(Acessos novoAcesso);
        Task AdicionarNovoVisAcessoAsync(VisAcesso novoVisAcesso);
        Task AdicionarNovoMorAcessoAsync(MorAcesso novoMorAcesso);

        Task AtualizarAcessoAsync(Acessos acesso);
        Task AtualizarVisAcessoAsync(VisAcesso visAcesso);
        //Logs
        Task AdicionarLogAcessoAsync(LogAcessos novoLog);
        //Outros
        Task<string> BuscarPathImagesAsync();
        Task<Autorizados> BuscarAutorizadoPorIdAsync(long idAutorizado);
    }
}
