using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Models;

namespace TipMolde.Infrastructure.DB
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Molde> Moldes { get; set; }
        public virtual DbSet<Fases_producao> Fases_Producaos { get; set; }
    }
}
