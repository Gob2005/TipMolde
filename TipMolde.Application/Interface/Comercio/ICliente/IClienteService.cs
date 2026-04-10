using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.ICliente
{
    public interface IClienteService
    {
        Task<PagedResult<Cliente>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<Cliente?> GetByIdAsync(int id);
        Task<Cliente?> GetClienteWithEncomendasAsync(int clienteId);
        Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Cliente>> SearchBySiglaAsync(string searchTerm);

        Task<Cliente> CreateAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id);
    }
}
