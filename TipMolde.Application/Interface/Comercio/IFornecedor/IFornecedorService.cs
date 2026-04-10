using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.IFornecedor
{
    public interface IFornecedorService
    {
        Task<PagedResult<Fornecedor>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<Fornecedor?> GetByIdAsync(int id);
        Task<IEnumerable<Fornecedor>> SearchByNameAsync(string searchTerm);

        Task<Fornecedor> CreateAsync(Fornecedor fornecedor);
        Task UpdateAsync(Fornecedor fornecedor);
        Task DeleteAsync(int id);
    }
}
