using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Condo.Models
{
    public class Login
    {
        [Key]
        public long idLogin { get; set; }

        public long idMorador { get; set; }

        public short? tentativasLogin { get; set; }

        public string? usuario { get; set; }

        [StringLength(25, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 25 caracteres.")]
        public string? senha { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string? email { get; set; }

        public string? tokenResetSenha { get; set; }
        public string? autoridadeLogin { get; set; }
        public DateTime? dtCriacao { get; set; }
        public DateTime? dtAlteracao { get; set; }
        public DateTime? dtExpiracaoToken { get; set; }
        public DateTime? dtUltimoLogin { get; set; }




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
        public string usuario { get; set; }
        public string senha { get; set; }
        public string ip { get; set; }
    }
    public class Moradores
    {
        [Key]
        public long IdMorador { get; set; }

        public string nomeMorador { get; set; }
        public string emailMorador { get; set; }
        public string statusMorador { get; set; }

    }
    public class MorVis
    {
        [Key]
        public long idMorVis { get; set; }

        public long idMorador { get; set; }
        public long idVisitante { get; set; }

        [ForeignKey("idVisitante")]
        public Visitantes Visitante { get; set; }
    }
}
