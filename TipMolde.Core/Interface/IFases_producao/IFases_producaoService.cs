using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IFases_producao
{
    public interface IFases_producaoService
    {
        Task<IEnumerable<Fases_producao>> GetAllFases_producaoAsync();
        Task<Fases_producao> GetFase_producaoByIdAsync(int id);
        Task<Fases_producao> CreateFase_producaoAsync(Fases_producao fp);
        Task UpdateFase_producaoAsync(Fases_producao fp);
        Task DeleteFase_producaoAsync(int id);
    }
}
