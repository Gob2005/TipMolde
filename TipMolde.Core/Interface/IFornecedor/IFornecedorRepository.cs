using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IFornecedor
{
    public interface IFornecedorRepository : IGenericRepository<Fornecedor>
    {
        Task<Fornecedor?> GetByNifAsync(string nif);
        Task<IEnumerable<Fornecedor>> SearchByNameAsync(string searchTerm);
    }
}
