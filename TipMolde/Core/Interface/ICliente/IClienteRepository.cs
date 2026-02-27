using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.ICliente
{
    public interface IClienteRepository : IGenericRepository<Cliente>
    {
        Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm);
    }
}
