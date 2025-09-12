using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Project_Condo.Models
{
    public class Festas
    {
        [Key]
        public long id { get; set; }

        public string? nomeFesta { get; set; }
        public long? idUsuario { get; set; }
        public string? tipoUsuario { get; set; }
        public DateTime de { get; set; }
        public DateTime ate { get; set; }
        public string? fotoConvite { get; set; }
        public short metodo { get; set; }
        public short diaTodoDe { get; set; }
        public short diaTodoAte { get; set; }
        public string? horaDe { get; set; }
        public string? horaAte { get; set; }

        [NotMapped]
        public List<int>? Visitantes { get; set; }
    }
    public class FestaVisitante
    {
        public long idFesta { get; set; }
        public long idVisitante { get; set; }
        public string? tipo { get; set; }
    }
}
