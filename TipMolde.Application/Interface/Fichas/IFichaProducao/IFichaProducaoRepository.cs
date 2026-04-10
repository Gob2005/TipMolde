using TipMolde.Domain.Entities.Fichas;

namespace TipMolde.Application.Interface.Fichas.IFichaProducao
{
    public interface IFichaProducaoRepository : IGenericRepository<FichaProducao, int>
    {
        Task<IEnumerable<FichaProducao>> GetByEncomendaMoldeIdAsync(int encomendaMoldeId);
        Task<FichaProducao?> GetByIdWithHeaderAsync(int id);
        Task<FichaProducao?> GetFLTByIdAsync(int id);
    }
}
