using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Producao.IFasesProducao
{
    public interface IFasesProducaoService
    {
        Task<PagedResult<FasesProducao>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<FasesProducao?> GetByIdAsync(int id);
        Task<FasesProducao> CreateAsync(FasesProducao fp);
        Task UpdateAsync(FasesProducao fp);
        Task DeleteAsync(int id);
    }
}
