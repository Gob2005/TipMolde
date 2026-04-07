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
        public virtual DbSet<EspecificacoesTecnicas> EspecificacoesTecnicas { get; set; }
        public virtual DbSet<Encomenda> Encomendas { get; set; }
        public virtual DbSet<EncomendaMolde> EncomendasMoldes { get; set; }
        public virtual DbSet<Peca> Pecas { get; set; }
        public virtual DbSet<Maquina> Maquinas { get; set; }
        public virtual DbSet<Fornecedor> Fornecedores { get; set; }
        public virtual DbSet<PedidoMaterial> PedidosMaterial { get; set; }
        public virtual DbSet<ItemPedidoMaterial> ItensPedidoMaterial { get; set; }
        public virtual DbSet<FasesProducao> Fases_Producaos { get; set; }
        public virtual DbSet<RegistosProducao> RegistosProducao { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.NIF)
                .IsUnique();

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Sigla)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Molde>()
                .HasIndex(m => m.Numero)
                .IsUnique();

            modelBuilder.Entity<Encomenda>()
                .HasIndex(e => e.NumeroEncomendaCliente)
                .IsUnique();

            modelBuilder.Entity<EspecificacoesTecnicas>()
                .HasOne(e => e.Molde)
                .WithOne(m => m.Especificacoes)
                .HasForeignKey<EspecificacoesTecnicas>(e => e.Molde_id);

            modelBuilder.Entity<EncomendaMolde>()
                .HasOne(em => em.Encomenda)
                .WithMany(e => e.EncomendasMoldes)
                .HasForeignKey(em => em.Encomenda_id);

            modelBuilder.Entity<EncomendaMolde>()
                .HasOne(em => em.Molde)
                .WithMany(m => m.EncomendasMoldes)
                .HasForeignKey(em => em.Molde_id);

            modelBuilder.Entity<Fornecedor>()
                .HasIndex(f => f.NIF)
                .IsUnique();

            modelBuilder.Entity<PedidoMaterial>()
                .HasMany(p => p.Itens)
                .WithOne(i => i.PedidoMaterial)
                .HasForeignKey(i => i.PedidoMaterial_id);

            modelBuilder.Entity<Maquina>()
                .HasKey(m => m.Maquina_id);

            modelBuilder.Entity<FasesProducao>()
                .HasIndex(f => f.Nome)
                .IsUnique();

            modelBuilder.Entity<RegistosProducao>()
                .HasOne(r => r.Maquina)
                .WithMany()
                .HasForeignKey(r => r.Maquina_id);
        }
    }
}
