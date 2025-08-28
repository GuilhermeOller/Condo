using System.ComponentModel.DataAnnotations;

namespace Acesso_Moradores_Visitantes.Models
{
    public class Visitantes
    {
        [Key]
        public long idVisitante { get; set; }

        public string nomeVisitante { get; set; }
        public string celularVisitante { get; set; }
        public string? emailVisitante { get; set; }

        //public ICollection<UsuarioVisitante> UsuarioVisitantes { get; set; }
    }
}
