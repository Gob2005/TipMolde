using TipMolde.Core.Models.Comercio;

namespace TipMolde.Core.Interface.Comercio.IFornecedor
{
    public interface IFornecedorService
    {
        Task<IEnumerable<Fornecedor>> GetAllAsync();
        Task<Fornecedor?> GetByIdAsync(int id);
        Task<IEnumerable<Fornecedor>> SearchByNameAsync(string searchTerm);

        Task<Fornecedor> CreateAsync(Fornecedor fornecedor);
        Task UpdateAsync(Fornecedor fornecedor);
        Task DeleteAsync(int id);
    }
}
