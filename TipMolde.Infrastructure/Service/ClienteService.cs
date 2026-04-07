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
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Task.FromResult(Enumerable.Empty<Cliente>());
            return _clienteRepository.SearchByNameAsync(searchTerm);
        }

        public Task<IEnumerable<Cliente>> SearchBySiglaAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Task.FromResult(Enumerable.Empty<Cliente>());
            return _clienteRepository.SearchBySiglaAsync(searchTerm);
        }

        public Task<Cliente?> GetClienteWithEncomendasAsync(int clienteId)
        {
            return _clienteRepository.GetClienteWithEncomendasAsync(clienteId);
        }

        public async Task<Cliente> CreateClienteAsync(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nome))
                throw new ArgumentException("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(cliente.NIF))
                throw new ArgumentException("NIF e obrigatorio.");
            if (string.IsNullOrWhiteSpace(cliente.Sigla))
                throw new ArgumentException("Sigla e obrigatoria.");

            var nifExists = await _clienteRepository.GetByNifAsync(cliente.NIF.Trim());
            if (nifExists != null)
                throw new ArgumentException("Ja existe cliente com este NIF.");

            var siglaExists = await _clienteRepository.GetBySiglaAsync(cliente.Sigla.Trim());
            if (siglaExists != null)
                throw new ArgumentException("Ja existe cliente com esta Sigla.");

            cliente.Nome = cliente.Nome.Trim();
            cliente.NIF = cliente.NIF.Trim();
            cliente.Sigla = cliente.Sigla.Trim();

            await _clienteRepository.AddAsync(cliente);
            return cliente;
        }
        public async Task UpdateClienteAsync(Cliente cliente)
        {
            var existing = await _clienteRepository.GetByIdAsync(cliente.Cliente_id);
            if (existing == null)
                throw new KeyNotFoundException($"Cliente com ID {cliente.Cliente_id} não encontrado.");

            if (!string.IsNullOrWhiteSpace(cliente.NIF) && cliente.NIF != existing.NIF)
            {
                var nifExists = await _clienteRepository.GetByNifAsync(cliente.NIF.Trim());
                if (nifExists != null && nifExists.Cliente_id != existing.Cliente_id)
                    throw new ArgumentException("Ja existe cliente com este NIF.");
            }

            if (!string.IsNullOrWhiteSpace(cliente.Sigla) && cliente.Sigla != existing.Sigla)
            {
                var siglaExists = await _clienteRepository.GetBySiglaAsync(cliente.Sigla.Trim());
                if (siglaExists != null && siglaExists.Cliente_id != existing.Cliente_id)
                    throw new ArgumentException("Ja existe cliente com esta Sigla.");
            }

            existing.Nome = string.IsNullOrWhiteSpace(cliente.Nome) ? existing.Nome : cliente.Nome.Trim();
            existing.Pais = string.IsNullOrWhiteSpace(cliente.Pais) ? existing.Pais : cliente.Pais.Trim();
            existing.Email = string.IsNullOrWhiteSpace(cliente.Email) ? existing.Email : cliente.Email.Trim();
            existing.Telefone = string.IsNullOrWhiteSpace(cliente.Telefone) ? existing.Telefone : cliente.Telefone.Trim();
            existing.NIF = string.IsNullOrWhiteSpace(cliente.NIF) ? existing.NIF : cliente.NIF.Trim();
            existing.Sigla = string.IsNullOrWhiteSpace(cliente.Sigla) ? existing.Sigla : cliente.Sigla.Trim();

            await _clienteRepository.UpdateAsync(existing);
        }

        public async Task DeleteClienteAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null) throw new KeyNotFoundException($"Cliente com ID {id} nao encontrado.");
            await _clienteRepository.DeleteAsync(id);
        }
    }
}
