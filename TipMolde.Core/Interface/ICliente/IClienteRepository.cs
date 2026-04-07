using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.ICliente
{
    public interface IClienteRepository : IGenericRepository<Cliente>
    {
        Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Cliente>> SearchBySiglaAsync(string searchTerm);
        Task<Cliente?> GetClienteWithEncomendasAsync(int clienteId);
        Task<Cliente?> GetByNifAsync(string nif);
        Task<Cliente?> GetBySiglaAsync(string sigla);
    }
}
