using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class MoldeRepository : GenericRepository<Molde>, IMoldeRepository
    {
        public MoldeRepository(ApplicationDbContext context) : base(context) { }

        public Task<Molde?> GetByIdWithSpecsAsync(int id) =>
            _context.Moldes
                .Include(m => m.Especificacoes)
                .FirstOrDefaultAsync(m => m.Molde_id == id);

        public Task<Molde?> GetByNumeroAsync(string numero) =>
            _context.Moldes.FirstOrDefaultAsync(m => m.Numero == numero);

        public async Task<IEnumerable<Molde>> GetByEncomendaIdAsync(int encomendaId)
        {
            return await _context.EncomendasMoldes
                .Where(em => em.Encomenda_id == encomendaId)
                .Select(em => em.Molde!)
                .Include(m => m.Especificacoes)
                .ToListAsync();
        }

        public async Task AddMoldeWithSpecsAndLinkAsync(Molde molde, EspecificacoesTecnicas specs, EncomendaMolde link)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            await _context.Moldes.AddAsync(molde);
            await _context.SaveChangesAsync();

            specs.Molde_id = molde.Molde_id;
            await _context.EspecificacoesTecnicas.AddAsync(specs);

            link.Molde_id = molde.Molde_id;
            await _context.EncomendasMoldes.AddAsync(link);

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
    }
}
