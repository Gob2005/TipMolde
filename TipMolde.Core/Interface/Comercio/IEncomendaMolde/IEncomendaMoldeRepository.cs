using TipMolde.Core.Models.Comercio;

namespace TipMolde.Core.Interface.Comercio.IEncomendaMolde
{
    public interface IEncomendaMoldeRepository : IGenericRepository<EncomendaMolde>
    {
        Task<IEnumerable<EncomendaMolde>> GetByEncomendaIdAsync(int encomendaId);
        Task<IEnumerable<EncomendaMolde>> GetByMoldeIdAsync(int moldeId);
    }
}

