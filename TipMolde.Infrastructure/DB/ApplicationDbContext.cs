using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Models;
using TipMolde.Core.Models.Comercio;
using TipMolde.Core.Models.Desenho;
using TipMolde.Core.Models.Fichas;
using TipMolde.Core.Models.Producao;

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
        public virtual DbSet<FasesProducao> Fases_Producao { get; set; }
        public virtual DbSet<RegistosProducao> RegistosProducao { get; set; }
        public virtual DbSet<Projeto> Projetos { get; set; }
        public virtual DbSet<Revisao> Revisoes { get; set; }
        public virtual DbSet<RegistoTempoProjeto> RegistosTempoProjeto { get; set; }
        public virtual DbSet<FichaProducao> FichasProducao { get; set; }
        public virtual DbSet<RegistoEnsaio> RegistosEnsaio { get; set; }
        public virtual DbSet<RegistoMelhoriaAlteracao> RegistosMelhoriaAlteracao { get; set; }
        public virtual DbSet<RegistoOcorrencia> RegistosOcorrencia { get; set; }


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

            modelBuilder.Entity<Encomenda>()
                .Property(e => e.Estado).HasConversion<string>().HasMaxLength(30);

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

            modelBuilder.Entity<Maquina>()
                .Property(m => m.Estado).HasConversion<string>().HasMaxLength(30);

            modelBuilder.Entity<FasesProducao>()
                .HasIndex(f => f.Nome)
                .IsUnique();

            modelBuilder.Entity<RegistosProducao>()
                .HasOne(r => r.Maquina)
                .WithMany()
                .HasForeignKey(r => r.Maquina_id);

            modelBuilder.Entity<RegistosProducao>()
                .Property(r => r.Estado_producao).HasConversion<string>().HasMaxLength(30);

            modelBuilder.Entity<ItemPedidoMaterial>()
                .HasKey(i => new { i.PedidoMaterial_id, i.Peca_id });

            modelBuilder.Entity<FichaProducao>()
                .Property(f => f.Tipo).HasConversion<string>().HasMaxLength(10);

            modelBuilder.Entity<RegistoOcorrencia>()
                .HasOne(r => r.FichaProducao)
                .WithMany(f => f.RegistosOcorrencia)
                .HasForeignKey(r => r.FichaProducao_id);

            modelBuilder.Entity<RegistoMelhoriaAlteracao>()
                .HasOne(r => r.FichaProducao)
                .WithMany(f => f.RegistosMelhoriaAlteracao)
                .HasForeignKey(r => r.FichaProducao_id);

            modelBuilder.Entity<RegistoEnsaio>()
                .HasOne(r => r.FichaProducao)
                .WithOne(f => f.RegistoEnsaio)
                .HasForeignKey<RegistoEnsaio>(r => r.FichaProducao_id);

            modelBuilder.Entity<Projeto>()
                .HasOne(p => p.Molde)
                .WithMany()
                .HasForeignKey(p => p.Molde_id);

            modelBuilder.Entity<Revisao>()
                .HasOne(r => r.Projeto)
                .WithMany(p => p.Revisoes)
                .HasForeignKey(r => r.Projeto_id);

            modelBuilder.Entity<RegistoTempoProjeto>()
                .HasOne(r => r.Projeto)
                .WithMany(p => p.RegistosTempo)
                .HasForeignKey(r => r.Projeto_id);
        }
    }
}
