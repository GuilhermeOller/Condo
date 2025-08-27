using Acesso_Moradores_Visitantes.Models;
using Microsoft.EntityFrameworkCore;

namespace Acesso_Moradores_Visitantes.Data
{
    public class AppDbContextMorador : DbContext
    {
        public AppDbContextMorador(DbContextOptions<AppDbContextMorador> options)
            : base(options)
        {
        }
        public DbSet<Moradores> tblMorador { get; set; }
    }
}
