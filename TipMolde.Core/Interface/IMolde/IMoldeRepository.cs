using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IMolde
{
    public interface IMoldeRepository : IGenericRepository<Molde>
    {
        Task<Molde?> GetByIdWithSpecsAsync(int id);
        Task<Molde?> GetByNumeroAsync(string numero);
        Task<IEnumerable<Molde>> GetByEncomendaIdAsync(int encomendaId);
        Task AddMoldeWithSpecsAndLinkAsync(Molde molde, EspecificacoesTecnicas specs, EncomendaMolde link);
    }
}
