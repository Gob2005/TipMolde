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
            throw new NotImplementedException();
        }
    }
}
