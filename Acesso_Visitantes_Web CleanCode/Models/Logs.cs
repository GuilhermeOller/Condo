using System.ComponentModel.DataAnnotations;

namespace Acesso_Moradores_Visitantes.Models
{
    public class LogLogin
    {
        [Key]
        public string id { get; set; }

        public string ip { get; set; }
        public long idLogin { get; set; }
        public DateTime data { get; set; }
        public short flgLogou { get; set; }
    }
    public class LogAcessos
    {
        [Key]
        public long id { get; set; }

        public string ip { get; set; }
        public long idLogin { get; set; }
        public long idVisitante { get; set; }
        public DateTime data { get; set; }
    }
    public class LogCadastro
    {
        [Key]
        public string id { get; set; }

        public string ip { get; set; }
        public string NomeUsuario { get; set; }
        public DateTime Data { get; set; }
    }
    public class LogResetPassword
    {
        [Key]
        public string Ip { get; set; }

        public string NomeUsuario { get; set; }
        public string Email { get; set; }
        public DateTime Data { get; set; }
    }
}
