using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acesso_Moradores_Visitantes.Models
{
    public class Login
    {
        [Key]
        public long Id { get; set; }

        public string? Usuario { get; set; }

        [StringLength(30, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 30 caracteres.")]
        public string? Senha { get; set; }

        public int? NivelAcesso { get; set; }
        public string? Status { get; set; }
        public int? TentativasLogin { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public DateTime? CriadoEm { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string? TokenResetSenha { get; set; }
        public DateTime? TokenExpiraEm { get; set; }
        public string? NomeMorador { get; set; }
        public string? Tipo { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string? Email { get; set; }

        [NotMapped]
        public string cod { get; set; }
        [NotMapped]
        public string ip { get; set; }
    }
    public class EnviarTokenDto()
    {
        public string Email { get; set; }
        public string Ip { get; set; }
    }
    public class LoginModel
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string ip { get; set; }
    }
    public class Moradores
    {
        [Key]
        public long IdMorador { get; set; }

        public string Morador { get; set; }
        public string email { get; set; }
        public string flgStatus { get; set; }

    }
    public class UsuarioVisitante
    {
        [Key]
        public long Id { get; set; }

        public long IdUsuario { get; set; }
        public string Tipo { get; set; }
        public long IdVisitante { get; set; }
        public DateTime DataCadastro { get; set; }

        [ForeignKey("IdVisitante")]
        public Visitantes Visitante { get; set; }
    }
}
