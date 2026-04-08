using TipMolde.Core.Models.Fichas;

namespace TipMolde.Core.Interface.Fichas.IFichaProducao
{
    public interface IFichaProducaoService
    {
        Task<IEnumerable<FichaProducao>> GetByEncomendaMoldeIdAsync(int encomendaMoldeId);
        Task<FichaProducao?> GetCompletaByIdAsync(int id);
        Task<FichaProducao> CreateAsync(FichaProducao ficha);
        Task DeleteAsync(int id);
    }
}
