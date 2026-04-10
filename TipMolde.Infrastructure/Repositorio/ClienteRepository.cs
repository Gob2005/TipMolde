using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class ClienteRepository : GenericRepository<Cliente, int>, IClienteRepository
    {
        public ClienteRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Cliente?> GetClienteWithEncomendasAsync(int clienteId)
        {
            return await _context.Clientes
                .AsNoTracking()
                .Include(c => c.Encomendas)
                .FirstOrDefaultAsync(c => c.Cliente_id == clienteId);
        }

        public Task<Cliente?> GetByNifAsync(string nif) =>
            _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.NIF == nif);

        public Task<Cliente?> GetBySiglaAsync(string sigla) =>
            _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Sigla == sigla);

        public async Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm)
        {
            var term = searchTerm.Trim().ToLower();
            return await _context.Clientes
                .AsNoTracking()
                .Where(c => c.Nome.ToLower().Contains(term))
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }
        public async Task<IEnumerable<Cliente>> SearchBySiglaAsync(string searchTerm)
        {
            var term = searchTerm.Trim().ToLower();
            return await _context.Clientes
                .AsNoTracking()
                .Where(c => c.Sigla.ToLower().Contains(term))
                .OrderBy(c => c.Sigla)
                .ToListAsync();
        }
    }
}
