using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Condo.Models
{
    public class LogLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }

        public string ip { get; set; }
        public long idMorador { get; set; }
        public DateTime data { get; set; }
        public short flgLogou { get; set; }
    }
    public class LogAcessos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        public string ip { get; set; }
        public long idMorador { get; set; }
        public long idVisitante { get; set; }
        public DateTime data { get; set; }
    }
    public class LogCadastro
    {
        [Key]
        public long id { get; set; }
        public long idMorador { get; set; }
        public string ip { get; set; }

        public DateTime data { get; set; }
    }
    public class LogResetPassword
    {
        [Key]
        public long id { get; set; }

        public string ip { get; set; }

        public string idMorador { get; set; }
        public string email { get; set; }
        public DateTime data { get; set; }
    }
}
