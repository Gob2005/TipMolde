using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IRegistosProducao
{
    public interface IRegistosProducaoRepository : IGenericRepository<RegistosProducao> 
    {
        Task<IEnumerable<RegistosProducao>> GetHistoricoAsync(int moldeId, int faseId, int pecaId);
        Task<RegistosProducao?> GetUltimoRegistoAsync(int moldeId, int faseId, int pecaId);
    }
}
