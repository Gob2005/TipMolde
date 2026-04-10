using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.IFornecedor
{
    public interface IFornecedorRepository : IGenericRepository<Fornecedor, int>
    {
        Task<Fornecedor?> GetByNifAsync(string nif);
        Task<IEnumerable<Fornecedor>> SearchByNameAsync(string searchTerm);
    }
}
