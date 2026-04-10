using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface.Desenho.IProjeto;
using TipMolde.Domain.Entities.Desenho;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class ProjetoRepository : GenericRepository<Projeto, int>, IProjetoRepository
    {
        public ProjetoRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Projeto>> GetByMoldeIdAsync(int moldeId)
        {
            return await _context.Projetos
                .AsNoTracking()
                .Where(p => p.Molde_id == moldeId)
                .OrderByDescending(p => p.Projeto_id)
                .ToListAsync();
        }

        public Task<Projeto?> GetWithRevisoesAsync(int id) =>
            _context.Projetos
                .Include(p => p.Revisoes)
                .FirstOrDefaultAsync(p => p.Projeto_id == id);
    }
}
