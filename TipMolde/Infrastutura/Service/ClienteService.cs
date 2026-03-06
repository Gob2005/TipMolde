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
            if (string.IsNullOrWhiteSpace(cliente.Nome)) throw new ArgumentException("O nome do cliente e obrigatorio.");
            if (string.IsNullOrWhiteSpace(cliente.NIF)) throw new ArgumentException("O NIF do cliente e obrigatorio.");
            await _clienteRepository.AddAsync(cliente);
            return cliente;
        }

        public async Task DeleteClienteAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null) throw new KeyNotFoundException($"Cliente com ID {id} nao encontrado.");
            await _clienteRepository.DeleteAsync(id);
        }

        public Task UpdateClienteAsync(Cliente cliente)
        {
            return _clienteRepository.UpdateAsync(cliente);
        }

        public Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            return _clienteRepository.GetAllAsync();
        }

        public Task<Cliente?> GetClienteByIdAsync(int id)
        {
            return _clienteRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm)
        {
            return _clienteRepository.SearchByNameAsync(searchTerm);
        }

        public Task<IEnumerable<Cliente>> SearchBySiglaAsync(string searchTerm)
        {
            return _clienteRepository.SearchBySiglaAsync(searchTerm);
        }
    }
}
