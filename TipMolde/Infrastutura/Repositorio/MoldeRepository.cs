using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Models;
using TipMolde.Infrastutura.DB;

namespace TipMolde.Infrastutura.Repositorio
{
    public class MoldeRepository : GenericRepository<Molde>, IMoldeRepository
    {
        public MoldeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Cliente> GetClienteByIdAsync(int clienteId)
        {
            return await _context.Clientes.FindAsync(clienteId);
        }
    }
}
