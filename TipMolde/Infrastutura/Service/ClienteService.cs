using TipMolde.Core.Interface.ICliente;
using TipMolde.Core.Models;

namespace TipMolde.Infrastutura.Service
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }
        public async Task<Cliente> CreateClienteAsync(Cliente cliente)
        {
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            if (string.IsNullOrEmpty(cliente.Nome)) throw new ArgumentException("O nome do cliente é obrigatório.");
            if (string.IsNullOrEmpty(cliente.NIF)) throw new ArgumentException("O NIF do cliente é obrigatório.");
            await _clienteRepository.AddAsync(cliente);
            return cliente;
        }
        public async Task DeleteClienteAsync(int id)
        {
            var cliente = _clienteRepository.GetByIdAsync(id);
            if (cliente == null) throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");
            await _clienteRepository.DeleteAsync(id);
        }
        public async Task UpdateClienteAsync(Cliente cliente)
        {
            await _clienteRepository.UpdateAsync(cliente);
        }
        public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            return await _clienteRepository.GetAllAsync();
        }
        public async Task<Cliente> GetClienteByIdAsync(int id)
        {
            return await _clienteRepository.GetByIdAsync(id);
        }
        public async Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm)
        {
            return await _clienteRepository.SearchByNameAsync(searchTerm);
        }
    }
}
