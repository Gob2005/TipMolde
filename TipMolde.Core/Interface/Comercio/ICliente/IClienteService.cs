using TipMolde.Core.Models.Comercio;

namespace TipMolde.Core.Interface.Comercio.ICliente
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllClientesAsync();
        Task<Cliente?> GetClienteByIdAsync(int id);
        Task<Cliente?> GetClienteWithEncomendasAsync(int clienteId);
        Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Cliente>> SearchBySiglaAsync(string searchTerm);

        Task<Cliente> CreateClienteAsync(Cliente cliente);
        Task UpdateClienteAsync(Cliente cliente);
        Task DeleteClienteAsync(int id);
    }
}
