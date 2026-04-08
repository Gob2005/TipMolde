using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.Comercio.IEncomendaMolde;
using TipMolde.Core.Models.Comercio;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class EncomendaMoldeRepository : GenericRepository<EncomendaMolde>, IEncomendaMoldeRepository
    {
        public EncomendaMoldeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<EncomendaMolde>> GetByEncomendaIdAsync(int encomendaId)
        {
            return await _context.EncomendasMoldes
                .AsNoTracking()
                .Include(em => em.Molde)
                .Where(em => em.Encomenda_id == encomendaId)
                .ToListAsync();
        }

        public async Task<IEnumerable<EncomendaMolde>> GetByMoldeIdAsync(int moldeId)
        {
            return await _context.EncomendasMoldes
                .AsNoTracking()
                .Include(em => em.Encomenda)
                .Where(em => em.Molde_id == moldeId)
                .ToListAsync();
        }
    }
}

