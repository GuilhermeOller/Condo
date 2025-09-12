using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Condo.Models
{
    public class Apartamento
    {
        [Key]
        public short id { get; set; }

        public string? nomeApartamento { get; set; }
        public string? telefoneApartamento { get; set; }
    }
    public class EmailConfig
    {
        [Key]
        public short id { get; set; }

        public string? smtpServer { get; set; }
        public string? smtpPort { get; set; }
        public string? email { get; set; }
        public string? senha { get; set; }
    }
    public class ZapiConfig
    {
        [Key]
        public short id { get; set; }

        public string? Instancia { get; set; }
        public string? TokenInstancia { get; set; }
        public string? TokenSeguranca { get; set; }
    }
    public class Parametros
    {
        [Key]
        public long idParametro { get; set; }

        public string? pathImagens { get; set; }
    }
}
