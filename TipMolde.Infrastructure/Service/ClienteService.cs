using TipMolde.Core.Interface.ICliente;
using TipMolde.Core.Models;

namespace TipMolde.Infrastructure.Service
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

        public async Task UpdateClienteAsync(Cliente cliente)
        {
            var existing = await _clienteRepository.GetByIdAsync(cliente.Cliente_id);
            if (existing == null)
                throw new KeyNotFoundException($"Cliente com ID {cliente.Cliente_id} não encontrado.");

            existing.Nome = string.IsNullOrWhiteSpace(cliente.Nome) ? existing.Nome : cliente.Nome.Trim();
            existing.Pais = string.IsNullOrWhiteSpace(cliente.Pais) ? existing.Pais : cliente.Pais.Trim();
            existing.Email = string.IsNullOrWhiteSpace(cliente.Email) ? existing.Email : cliente.Email.Trim();
            existing.Telefone = string.IsNullOrWhiteSpace(cliente.Telefone) ? existing.Telefone : cliente.Telefone.Trim();
            existing.NIF = string.IsNullOrWhiteSpace(cliente.NIF) ? existing.NIF : cliente.NIF.Trim();
            existing.Sigla = string.IsNullOrWhiteSpace(cliente.Sigla) ? existing.Sigla : cliente.Sigla.Trim();

            await _clienteRepository.UpdateAsync(existing);
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
