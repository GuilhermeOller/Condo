using Acesso_Moradores_Visitantes.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Acessos> tblAcessos { get; set; }

    public DbSet<Visitantes> tblVisitantes { get; set; }
    public DbSet<Moradores> tblMoradores { get; set; }
    public DbSet<UsuarioVisitante> tblMorVis { get; set; }
    public DbSet<LogLogin> tblLogLogin { get; set; }
    public DbSet<LogAcessos> tblLogAcessos { get; set; }
    public DbSet<Festas> tblFestas { get; set; }
    public DbSet<FestaVisitante> tblFestaVisitante { get; set; }
    public DbSet<Apartamento> tblApartamento { get; set; }
    public DbSet<Login> tblLoginMorador { get; set; }
    public DbSet<EmailConfig> tblEmailConfig { get; set; }
    public DbSet<LogCadastro> tblLogWebCadastro { get; set; }
    public DbSet<LogResetPassword> tblLogWebResetPassword { get; set; }
    public DbSet<ZapiConfig> tblZapiConfig { get; set; }
    public DbSet<Parametros> tblParametros { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FestaVisitante>()
            .HasKey(fv => new { fv.idFesta, fv.idVisitante });
    }
}
