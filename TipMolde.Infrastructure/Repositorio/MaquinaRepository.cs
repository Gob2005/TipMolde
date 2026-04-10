using Microsoft.EntityFrameworkCore;
using TipMolde.Domain.Enums;
using TipMolde.Application.Interface.Producao.IMaquina;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class MaquinaRepository : GenericRepository<Maquina, int>, IMaquinaRepository
    {
        public MaquinaRepository(ApplicationDbContext context) : base(context) { }

        public Task<Maquina?> GetByIdUnicoAsync(int id) =>
            _context.Maquinas.FirstOrDefaultAsync(m => m.Maquina_id == id);
        public Task<IEnumerable<Maquina>> GetByEstadoAsync(EstadoMaquina estado) =>
            _context.Maquinas.Where(m => m.Estado == estado)
            .ToListAsync().ContinueWith(t => t.Result.AsEnumerable());
    }
}
