using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.Fichas.IFichaProducao;
using TipMolde.Core.Models.Fichas;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class FichaProducaoRepository : GenericRepository<FichaProducao>, IFichaProducaoRepository
    {
        public FichaProducaoRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<FichaProducao>> GetByEncomendaMoldeIdAsync(int encomendaMoldeId)
        {
            return await _context.FichasProducao
                .AsNoTracking()
                .Where(f => f.EncomendaMolde_id == encomendaMoldeId)
                .OrderByDescending(f => f.DataGeracao)
                .ToListAsync();
        }

        public Task<FichaProducao?> GetByIdWithHeaderAsync(int id) =>
            _context.FichasProducao
                .Include(f => f.EncomendaMolde)
                    .ThenInclude(em => em.Encomenda)
                        .ThenInclude(e => e.Cliente)
                .Include(f => f.EncomendaMolde)
                    .ThenInclude(em => em.Molde)
                .FirstOrDefaultAsync(f => f.FichaProducao_id == id);

        public Task<FichaProducao?> GetFLTByIdAsync(int id) =>
            _context.FichasProducao
                .Include(f => f.EncomendaMolde)
                    .ThenInclude(em => em.Encomenda)
                        .ThenInclude(e => e.Cliente)
                .Include(f => f.EncomendaMolde)
                    .ThenInclude(em => em.Molde)
                        .ThenInclude(m => m.Especificacoes)
                .FirstOrDefaultAsync(f => f.FichaProducao_id == id);
    }
}