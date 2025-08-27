using Acesso_Moradores_Visitantes.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Acessos> tblAcessos { get; set; }

    public DbSet<Visitantes> tblVisitante { get; set; }
    public DbSet<Moradores> tblMorador { get; set; }
    public DbSet<Autorizados> tblAutorizados { get; set; }
    public DbSet<VisAcesso> tblVisAcesso { get; set; }
    public DbSet<MorAcesso> tblMorAcesso { get; set; }
    public DbSet<UsuarioVisitante> tblUsuarioVisitante { get; set; }
    public DbSet<LogLogin> tblLogWebLogin { get; set; }
    public DbSet<LogAcessos> tblLogWebAcessos { get; set; }
    public DbSet<Festas> tblFestas { get; set; }
    public DbSet<FestaVisitante> tblFestaVisitante { get; set; }
    public DbSet<Condominio> tblEmpresa { get; set; }
    public DbSet<Login> tblLoginMorador { get; set; }
    public DbSet<Email> tblEmail { get; set; }
    public DbSet<LogCadastro> tblLogWebCadastro { get; set; }
    public DbSet<LogResetPassword> tblLogWebResetPassword { get; set; }
    public DbSet<Zapi> tblZAPI { get; set; }
    public DbSet<Parametros> tblParametros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FestaVisitante>()
            .HasKey(fv => new { fv.idFesta, fv.idVisitante });
    }
}
