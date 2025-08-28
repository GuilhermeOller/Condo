using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acesso_Moradores_Visitantes.Models
{
    public class Acessos
    {
        [Key]
        public long idAcesso { get; set; }

        public int numAcesso { get; set; }
        public long idVisitante { get; set; }
        public long idMorador { get; set; }
        public DateTime de { get; set; }
        public DateTime ate { get; set; }
        public DateTime? entrada { get; set; }
        public DateTime? saida { get; set; }


        [ForeignKey("IdOrigem")]
        public Visitantes VisitanteInfo { get; set; }

        [NotMapped]
        public string ip { get; set; }
    }
}
