using TipMolde.Core.Interface.Comercio.IFornecedor;
using TipMolde.Core.Models.Comercio;

namespace TipMolde.Infrastructure.Service
{
    public class FornecedorService : IFornecedorService
    {
        private readonly IFornecedorRepository _fornecedorRepository;

        public FornecedorService(IFornecedorRepository fornecedorRepository)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        public Task<IEnumerable<Fornecedor>> GetAllAsync() => _fornecedorRepository.GetAllAsync();

        public Task<Fornecedor?> GetByIdAsync(int id) => _fornecedorRepository.GetByIdAsync(id);

        public Task<IEnumerable<Fornecedor>> SearchByNameAsync(string searchTerm) =>
            string.IsNullOrWhiteSpace(searchTerm)
                ? Task.FromResult(Enumerable.Empty<Fornecedor>())
                : _fornecedorRepository.SearchByNameAsync(searchTerm);

        public async Task<Fornecedor> CreateAsync(Fornecedor fornecedor)
        {
            if (string.IsNullOrWhiteSpace(fornecedor.Nome))
                throw new ArgumentException("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(fornecedor.NIF))
                throw new ArgumentException("NIF e obrigatorio.");

            var existing = await _fornecedorRepository.GetByNifAsync(fornecedor.NIF.Trim());
            if (existing != null)
                throw new ArgumentException("Ja existe fornecedor com este NIF.");

            fornecedor.Nome = fornecedor.Nome.Trim();
            fornecedor.NIF = fornecedor.NIF.Trim();

            await _fornecedorRepository.AddAsync(fornecedor);
            return fornecedor;
        }

        public async Task UpdateAsync(Fornecedor fornecedor)
        {
            var existing = await _fornecedorRepository.GetByIdAsync(fornecedor.Fornecedor_id);
            if (existing == null)
                throw new KeyNotFoundException($"Fornecedor com ID {fornecedor.Fornecedor_id} nao encontrado.");

            if (!string.IsNullOrWhiteSpace(fornecedor.NIF) && fornecedor.NIF != existing.NIF)
            {
                var nifExists = await _fornecedorRepository.GetByNifAsync(fornecedor.NIF.Trim());
                if (nifExists != null && nifExists.Fornecedor_id != existing.Fornecedor_id)
                    throw new ArgumentException("Ja existe fornecedor com este NIF.");
            }

            existing.Nome = string.IsNullOrWhiteSpace(fornecedor.Nome) ? existing.Nome : fornecedor.Nome.Trim();
            existing.NIF = string.IsNullOrWhiteSpace(fornecedor.NIF) ? existing.NIF : fornecedor.NIF.Trim();
            existing.Morada = string.IsNullOrWhiteSpace(fornecedor.Morada) ? existing.Morada : fornecedor.Morada.Trim();
            existing.Email = string.IsNullOrWhiteSpace(fornecedor.Email) ? existing.Email : fornecedor.Email.Trim();
            existing.Telefone = string.IsNullOrWhiteSpace(fornecedor.Telefone) ? existing.Telefone : fornecedor.Telefone.Trim();

            await _fornecedorRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var fornecedor = await _fornecedorRepository.GetByIdAsync(id);
            if (fornecedor == null)
                throw new KeyNotFoundException($"Fornecedor com ID {id} nao encontrado.");
            await _fornecedorRepository.DeleteAsync(id);
        }
    }
}
