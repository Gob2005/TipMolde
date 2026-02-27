using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.ICliente;
using TipMolde.Core.Models;
using TipMolde.Infrastutura.DB;

namespace TipMolde.Infrastutura.Repositorio
{
    public class ClienteRepository : GenericRepository<Cliente>, IClienteRepository
    {
        public ClienteRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<Cliente>();
            }

            var term = $"%{searchTerm.Trim()}%";
            return await _context.Clientes
                .AsNoTracking()
                .Where(c => EF.Functions.Like(c.Nome, term))
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }
    }
}
