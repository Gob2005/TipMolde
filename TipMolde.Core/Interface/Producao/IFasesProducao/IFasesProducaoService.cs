using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Interface.Producao.IFasesProducao
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
