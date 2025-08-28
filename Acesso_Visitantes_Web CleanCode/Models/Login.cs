using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acesso_Moradores_Visitantes.Models
{
    public class Login
    {
        [Key]
        public long idLogin { get; set; }

        public long idMorador { get; set; }

        public string? usuario { get; set; }

        [StringLength(25, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 25 caracteres.")]
        public string? senha { get; set; }

        public DateTime? dtCriacao { get; set; }
        public DateTime? dtAlteracao { get; set; }
        public DateTime? dtExpiracaoToken { get; set; }
        public string? tokenResetSenha { get; set; }
        public DateTime? ultimoLogin { get; set; }
        public int? tentativasLogin { get; set; }
        public string? NomeMorador { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string? email { get; set; }



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
