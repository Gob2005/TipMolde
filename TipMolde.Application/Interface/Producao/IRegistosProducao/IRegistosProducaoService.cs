using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Producao.IRegistosProducao
{
    public interface IRegistosProducaoService
    {
        Task<PagedResult<RegistosProducao>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<RegistosProducao?> GetByIdAsync(int id);
        Task<IEnumerable<RegistosProducao>> GetHistoricoAsync(int faseId, int pecaId);
        Task<RegistosProducao?> GetUltimoRegistoAsync(int faseId, int pecaId);

        Task<RegistosProducao> CreateAsync(RegistosProducao registo);
    }
}
