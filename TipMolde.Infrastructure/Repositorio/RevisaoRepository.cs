using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.Desenho.IRevisao;
using TipMolde.Core.Models.Desenho;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class RevisaoRepository : GenericRepository<Revisao>, IRevisaoRepository
    {
        public RevisaoRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Revisao>> GetByProjetoIdAsync(int projetoId)
        {
            return await _context.Revisoes
                .AsNoTracking()
                .Where(r => r.Projeto_id == projetoId)
                .OrderByDescending(r => r.NumRevisao)
                .ToListAsync();
        }

        public async Task<int> GetNextNumeroRevisaoAsync(int projetoId)
        {
            var max = await _context.Revisoes
                .Where(r => r.Projeto_id == projetoId)
                .Select(r => (int?)r.NumRevisao)
                .MaxAsync();

            return (max ?? 0) + 1;
        }
    }
}
