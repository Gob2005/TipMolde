using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.ICliente
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllClientesAsync();
        Task<Cliente?> GetClienteByIdAsync(int id);
        Task<Cliente?> GetClienteWithEncomendasAsync(int clienteId);
        Task<Cliente> CreateClienteAsync(Cliente cliente);
        Task UpdateClienteAsync(Cliente cliente);
        Task DeleteClienteAsync(int id);
        Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Cliente>> SearchBySiglaAsync(string searchTerm);
    }
}
