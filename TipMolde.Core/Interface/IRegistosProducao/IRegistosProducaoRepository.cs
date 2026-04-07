using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IRegistosProducao
{
    public interface IRegistosProducaoRepository : IGenericRepository<RegistosProducao> 
    {
        Task<IEnumerable<RegistosProducao>> GetHistoricoAsync(int faseId, int pecaId);
        Task<RegistosProducao?> GetUltimoRegistoAsync(int faseId, int pecaId);
        Task<IEnumerable<RegistosProducao>> GetByMaquinaAsync(int maquinaId);
    }
}
