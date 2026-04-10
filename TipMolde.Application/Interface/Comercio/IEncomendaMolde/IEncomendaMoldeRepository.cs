using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.IEncomendaMolde
{
    public interface IEncomendaMoldeRepository : IGenericRepository<EncomendaMolde, int>
    {
        Task<IEnumerable<EncomendaMolde>> GetByEncomendaIdAsync(int encomendaId);
        Task<IEnumerable<EncomendaMolde>> GetByMoldeIdAsync(int moldeId);
    }
}

