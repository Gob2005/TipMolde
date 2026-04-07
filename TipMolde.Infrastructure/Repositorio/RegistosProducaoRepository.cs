using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.IRegistosProducao;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class RegistosProducaoRepository : GenericRepository<RegistosProducao>, IRegistosProducaoRepository
    {
        public RegistosProducaoRepository(ApplicationDbContext context) : base(context)   {  }
        public Task<IEnumerable<RegistosProducao>> GetHistoricoAsync(int faseId, int pecaId)
        {
            return Task.FromResult<IEnumerable<RegistosProducao>>(
                _context.RegistosProducao
                    .AsNoTracking()
                    .Where(r => r.Fase_id == faseId
                         && r.Peca_id == pecaId)
                    .OrderBy(r => r.Data_hora)
                    .ToList()
            );
        }

        public Task<RegistosProducao?> GetUltimoRegistoAsync(int faseId, int pecaId)
        {
            return _context.RegistosProducao
                .AsNoTracking()
                .Where(r => r.Fase_id == faseId
                         && r.Peca_id == pecaId)
                .OrderByDescending(r => r.Data_hora)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RegistosProducao>> GetByMaquinaAsync(int maquinaId)
        {
            return await _context.RegistosProducao
                .Where(r => r.Maquina_id == maquinaId)
                .OrderByDescending(r => r.Data_hora)
                .ToListAsync();
        }
    }
}
