using TipMolde.Core.Models.Comercio;

namespace TipMolde.Core.Interface.Comercio.IFornecedor
{
    public interface IFornecedorRepository : IGenericRepository<Fornecedor>
    {
        Task<Fornecedor?> GetByNifAsync(string nif);
        Task<IEnumerable<Fornecedor>> SearchByNameAsync(string searchTerm);
    }
}
