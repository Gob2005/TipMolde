using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TipMolde.Core.Models;

namespace TipMolde.Infrastutura.DB
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
    }
}
