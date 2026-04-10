using TipMolde.Domain.Entities.Fichas;

namespace TipMolde.Application.Interface.Fichas.IFichaProducao
{
    public interface IFichaProducaoService
    {
        Task<IEnumerable<FichaProducao>> GetByEncomendaMoldeIdAsync(int encomendaMoldeId);
        Task<FichaProducao?> GetByIdWithHeaderAsync(int id);
        Task<FichaProducao?> GetFLTByIdAsync(int id);

        Task<FichaProducao> CreateAsync(FichaProducao ficha);
        Task DeleteAsync(int id);
    }
}
