using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acesso_Moradores_Visitantes.Models
{
    public class Condominio
    {
        [Key]
        public short IdEmpresa { get; set; }

        public string? Empresa { get; set; }
        public string? Fantazia { get; set; }
        public string? FlgPessoa { get; set; }
        public string? Docto { get; set; }
        public string? Sistema { get; set; }
        public short? nEmp { get; set; }
        public short? Copias { get; set; }
        public DateTime? DtAquisicao { get; set; }
        public string? Renovacao { get; set; }
        public string? Chave { get; set; }
        public DateTime? Vencimento { get; set; }
        public string? ContraChave { get; set; }
        public byte[]? Logo { get; set; }
        public string? endereco { get; set; }
        public string? numero { get; set; }
        public string? complemento { get; set; }
        public string? bairro { get; set; }
        public string? cidade { get; set; }
        public string? uf { get; set; }
        public string? cep { get; set; }
        public string? fone { get; set; }
        public string? fax { get; set; }
        public string? mail { get; set; }
        public DateTime? Acesso { get; set; }
        public string? A1 { get; set; }
        public string? A2 { get; set; }
        public string? IE { get; set; }
        public decimal? vLAc { get; set; }
        public decimal? vLAt { get; set; }
        public decimal? FixoAc { get; set; }
        public decimal? FixoAt { get; set; }
        public string? url { get; set; }
        public string? cod { get; set; }
    }
    public class Email
    {
        public short? ID { get; set; }
        public string? smtpServer { get; set; }
        public string? smtpPort { get; set; }
        public string? email { get; set; }
        public string? senha { get; set; }
    }
    public class Zapi
    {
        public string? Instancia { get; set; }
        public string? TokenInstancia { get; set; }
        public string? TokenSeguranca { get; set; }
        public string? ID { get; set; }
        public string? HeaderMsg { get; set; }
        public string? BodyMsg { get; set; }
        public string? WaMe { get; set; }
    }
    public class Parametros
    {
        [Key]
        public long IdParametro { get; set; }

        public string? PathImages { get; set; }
    }
}
