using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IMolde
{
    public interface IMoldeService
    {
        Task<IEnumerable<Molde>> GetAllMoldesAsync();
        Task<Molde?> GetMoldeByIdAsync(int id);
        Task<Molde?> GetMoldeWithSpecsAsync(int id);
        Task<Molde?> GetByNumeroAsync(string numero);
        Task<IEnumerable<Molde>> GetByEncomendaIdAsync(int encomendaId);

        Task<Molde> CreateMoldeAsync(Molde molde, EspecificacoesTecnicas specs, EncomendaMolde link);
        Task UpdateMoldeAsync(Molde molde, EspecificacoesTecnicas? specs);
        Task DeleteMoldeAsync(int id);
    }
}
