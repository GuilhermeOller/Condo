using System.ComponentModel.DataAnnotations;

namespace Project_Condo.Models
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
