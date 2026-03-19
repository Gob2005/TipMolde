using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.IRegistosProducao;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class RegistosProducaoRepository : GenericRepository<RegistosProducao>, IRegistosProducaoRepository
    {
        public RegistosProducaoRepository(ApplicationDbContext context) : base(context)   {  }
        public Task<IEnumerable<RegistosProducao>> GetHistoricoAsync(int moldeId, int faseId, int pecaId)
        {
            return Task.FromResult<IEnumerable<RegistosProducao>>(
                _context.RegistosProducao
                    .AsNoTracking()
                    .Where(r => r.Molde_id == moldeId
                             && r.Fase_id == faseId
                             && r.Peca_id == pecaId)
                    .OrderBy(r => r.Data_hora)
                    .ToList()
            );
        }

        public Task<RegistosProducao?> GetUltimoRegistoAsync(int moldeId, int faseId, int pecaId)
        {
            return _context.RegistosProducao
                .AsNoTracking()
                .Where(r => r.Molde_id == moldeId
                         && r.Fase_id == faseId
                         && r.Peca_id == pecaId)
                .OrderByDescending(r => r.Data_hora)
                .FirstOrDefaultAsync();
        }
    }
}
