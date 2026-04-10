using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface.Desenho.IRegistoTempoProjeto;
using TipMolde.Domain.Entities.Desenho;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class RegistoTempoProjetoRepository : GenericRepository<RegistoTempoProjeto, int>, IRegistoTempoProjetoRepository
    {
        public RegistoTempoProjetoRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<RegistoTempoProjeto>> GetHistoricoAsync(int projetoId, int autorId)
        {
            return await _context.RegistosTempoProjeto
                .AsNoTracking()
                .Where(r => r.Projeto_id == projetoId && r.Autor_id == autorId)
                .OrderBy(r => r.Data_hora)
                .ToListAsync();
        }

        public Task<RegistoTempoProjeto?> GetUltimoRegistoAsync(int projetoId, int autorId)
        {
            return _context.RegistosTempoProjeto
                .AsNoTracking()
                .Where(r => r.Projeto_id == projetoId && r.Autor_id == autorId)
                .OrderByDescending(r => r.Data_hora)
                .FirstOrDefaultAsync();
        }
    }
}
