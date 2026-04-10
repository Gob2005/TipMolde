using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.ICliente
{
    public interface IClienteRepository : IGenericRepository<Cliente, int>
    {
        Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Cliente>> SearchBySiglaAsync(string searchTerm);
        Task<Cliente?> GetClienteWithEncomendasAsync(int clienteId);
        Task<Cliente?> GetByNifAsync(string nif);
        Task<Cliente?> GetBySiglaAsync(string sigla);
    }
}
