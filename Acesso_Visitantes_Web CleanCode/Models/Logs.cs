using System.ComponentModel.DataAnnotations;

namespace Acesso_Moradores_Visitantes.Models
{
    public class LogLogin
    {
        [Key]
        public string Ip { get; set; }

        public string Nome { get; set; }
        public DateTime Data { get; set; }
        public short Logou { get; set; }
    }
    public class LogAcessos
    {
        [Key]
        public long id { get; set; }

        public string Ip { get; set; }
        public string NomeUsuario { get; set; }
        public string NomeVisitante { get; set; }
        public DateTime Data { get; set; }
    }
    public class LogCadastro
    {
        [Key]
        public string Ip { get; set; }

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
