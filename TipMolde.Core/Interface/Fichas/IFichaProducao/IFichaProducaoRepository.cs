using TipMolde.Core.Models.Fichas;

namespace TipMolde.Core.Interface.Fichas.IFichaProducao
{
    public interface IFichaProducaoRepository : IGenericRepository<FichaProducao>
    {
        Task<IEnumerable<FichaProducao>> GetByEncomendaMoldeIdAsync(int encomendaMoldeId);
        Task<FichaProducao?> GetCompletaByIdAsync(int id);
    }
}
