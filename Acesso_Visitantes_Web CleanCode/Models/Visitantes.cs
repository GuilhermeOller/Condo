using System.ComponentModel.DataAnnotations;

namespace Acesso_Moradores_Visitantes.Models
{
    public class Visitantes
    {
        [Key]
        public long idVisitante { get; set; }

        public string Visitante { get; set; }
        public string Empresa { get; set; }
        public string FoneCelular { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public string TpDocto { get; set; }
        public string NumDocto { get; set; }
        public DateTime? DtNascimento { get; set; }
        public string Placa { get; set; }
        public string Veiculo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Cor { get; set; }
        public string FlgStatus { get; set; }
        public string FlgTipo { get; set; }
        public short temCarro { get; set; }
        public string? email { get; set; }

        //public ICollection<UsuarioVisitante> UsuarioVisitantes { get; set; }
    }
}
