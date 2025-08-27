using System.ComponentModel.DataAnnotations;

namespace Acesso_Moradores_Visitantes.Models
{
    public class Autorizados
    {
        [Key]
        public long IdAutorizado { get; set; }

        public long IdMorador { get; set; }

        public string Nome { get; set; }
        public string email { get; set; }
        public short flgExcluido { get; set; }

        //public ICollection<UsuarioVisitante> MoradorVisitantes { get; set; }

    }
}
