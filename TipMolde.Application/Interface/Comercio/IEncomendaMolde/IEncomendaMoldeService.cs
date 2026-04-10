using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.IEncomendaMolde
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

