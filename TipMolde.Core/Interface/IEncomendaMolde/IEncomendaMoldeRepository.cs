using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IEncomendaMolde
{
    public interface IEncomendaMoldeRepository : IGenericRepository<EncomendaMolde>
    {
        Task<IEnumerable<EncomendaMolde>> GetByEncomendaIdAsync(int encomendaId);
        Task<IEnumerable<EncomendaMolde>> GetByMoldeIdAsync(int moldeId);
    }
}

