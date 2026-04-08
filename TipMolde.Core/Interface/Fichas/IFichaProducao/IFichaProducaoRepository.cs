using TipMolde.Core.Models.Fichas;

namespace TipMolde.Core.Interface.Fichas.IFichaProducao
{
    public interface IFichaProducaoRepository : IGenericRepository<FichaProducao>
    {
        Task<IEnumerable<FichaProducao>> GetByEncomendaMoldeIdAsync(int encomendaMoldeId);
        Task<FichaProducao?> GetByIdWithHeaderAsync(int id);
        Task<FichaProducao?> GetFLTByIdAsync(int id);
    }
}
