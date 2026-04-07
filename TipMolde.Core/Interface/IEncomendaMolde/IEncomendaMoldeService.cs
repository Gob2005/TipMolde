using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IEncomendaMolde
{
    public interface IEncomendaMoldeService
    {
        Task<IEnumerable<EncomendaMolde>> GetByEncomendaIdAsync(int encomendaId);
        Task<IEnumerable<EncomendaMolde>> GetByMoldeIdAsync(int moldeId);
        Task<EncomendaMolde> CreateAsync(EncomendaMolde link);
        Task UpdateAsync(EncomendaMolde link);
        Task DeleteAsync(int id);
    }
}

