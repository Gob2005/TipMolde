using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IRegistosProducao
{
    public interface IRegistosProducaoService
    {
        Task<IEnumerable<RegistosProducao>> GetAllRegistosProducaoAsync();
        Task<RegistosProducao?> GetRegistoProducaoByIdAsync(int id);
        Task<RegistosProducao> CreateRegistoProducaoAsync(RegistosProducao registo);
        Task DeleteRegistoProducaoAsync(int id);
        Task<IEnumerable<RegistosProducao>> GetHistoricoAsync(int moldeId, int faseId, int pecaId);
        Task<RegistosProducao?> GetUltimoRegistoAsync(int moldeId, int faseId, int pecaId);
    }
}
