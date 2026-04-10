using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Producao.IMolde
{
    public interface IMoldeRepository : IGenericRepository<Molde, int>
    {
        Task<Molde?> GetByIdWithSpecsAsync(int id);
        Task<Molde?> GetByNumeroAsync(string numero);
        Task<IEnumerable<Molde>> GetByEncomendaIdAsync(int encomendaId);
        Task AddMoldeWithSpecsAsync(Molde molde, EspecificacoesTecnicas specs);
        Task AddMoldeWithSpecsAndLinkAsync(Molde molde, EspecificacoesTecnicas specs, EncomendaMolde link);
    }
}
