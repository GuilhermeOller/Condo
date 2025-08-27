using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acesso_Moradores_Visitantes.Models
{
    public class Acessos
    {
        [Key]
        public long IdAcesso { get; set; }

        public int NumCracha { get; set; }
        public byte? CrachaLiberado { get; set; }
        public string FlgOrigem { get; set; }
        public long IdVisitado { get; set; }
        public string FlgPortao { get; set; }
        public string FlgTpAcesso { get; set; }
        public DateTime? DtEntrada { get; set; }
        public DateTime? DtSaida { get; set; }
        public DateTime DtAlteracao { get; set; }
        public short IdOperador { get; set; }
        public byte? FlgExcluido { get; set; }
        public short PortaoAcesso { get; set; }
        public byte? entrada { get; set; }
        public byte? saida { get; set; }
        public byte? caminhao { get; set; }
        public long IdOrigem { get; set; }
        public DateTime de { get; set; }
        public DateTime ate { get; set; }
        public string TipoVisitado { get; set; }

        [ForeignKey("IdOrigem")]
        public Visitantes VisitanteInfo { get; set; }

        [NotMapped]
        public string ip { get; set; }
    }
    public class MorAcesso
    {
        [Key]
        public long IdReg { get; set; }

        public long IdAcesso { get; set; }
        public long IdMorador { get; set; }
        public string Morador { get; set; }

    }
    public class VisAcesso
    {
        [Key]
        public long IdReg { get; set; }

        public long IdAcesso { get; set; }
        public string FlgOrigem { get; set; }
        public long IdOrigem { get; set; }
        public byte FlgCondutor { get; set; }
        public DateTime? DtSaida { get; set; }
        public string FlgStatusAcesso { get; set; }
        public string Visitante { get; set; }
        public int numcracha { get; set; }
    }
}
