using Project_Condo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Project_Condo.Services.Interfaces
{
    public interface IVisitanteService
    {
        Task <(bool Sucesso, string Mensagem, long idVisitante)> AdcionarVisitanteAsync(Visitantes visitante, long idUsuario, string tipoLogin);
        Task <(bool Sucesso, string Mensagem)> UploadFotoAsync(IFormCollection form);
        Task <(bool Sucesso, string Mensagem)> ExcluirVisitanteAsync(long idVisitante, long idUsuario, string tipoLogin);
        Task <(bool Sucesso, string Mensagem)> LiberarVisitanteAsync([FromBody] Acessos acesso, string tipo, string nomeFesta, short metodo,
            string convite, string tipoVisitante, long idUsuario, string tipoLogin, string nomeLogin);
        Task<(bool Sucesso, string Mensagem)> BaixarAcessoAsync(long idAcesso);
        Task<(bool Sucesso, string Mensagem)> CadastrarFestaAsync(Festas festa, long idUsuario, string tipoLogin, string nomeLogin);
        Task<(bool Sucesso, string Mensagem)> ExcluirFestaAsync(long idFesta, long idUsuario);
        Task<(bool Sucesso, string Mensagem)> AtualizarFestaAsync(Festas festa, long idUsuario, string tipoLogin, string nomeLogin);
    }
}
