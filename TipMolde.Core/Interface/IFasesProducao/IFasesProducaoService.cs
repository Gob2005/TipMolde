using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IFases_producao
{
    public interface IFasesProducaoService
    {
        Task<IEnumerable<FasesProducao>> GetAllFases_producaoAsync();
        Task<FasesProducao?> GetFase_producaoByIdAsync(int id);
        Task<FasesProducao> CreateFase_producaoAsync(FasesProducao fp);
        Task UpdateFase_producaoAsync(FasesProducao fp);
        Task DeleteFase_producaoAsync(int id);
    }
}
