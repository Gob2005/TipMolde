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
        public virtual DbSet<Encomenda> Encomendas { get; set; }
        public virtual DbSet<Peca> Pecas { get; set; }
        public virtual DbSet<Fornecedor> Fornecedores { get; set; }
        public virtual DbSet<FasesProducao> Fases_Producaos { get; set; }
        public virtual DbSet<RegistosProducao> RegistosProducao { get; set; }
    }
}
