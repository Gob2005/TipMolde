using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Producao.IRegistosProducao
{
    public interface IRegistosProducaoRepository : IGenericRepository<RegistosProducao, int>
    {
        Task<IEnumerable<RegistosProducao>> GetHistoricoAsync(int faseId, int pecaId);
        Task<RegistosProducao?> GetUltimoRegistoAsync(int faseId, int pecaId);
        Task<IEnumerable<RegistosProducao>> GetByMaquinaAsync(int maquinaId);
    }
}
