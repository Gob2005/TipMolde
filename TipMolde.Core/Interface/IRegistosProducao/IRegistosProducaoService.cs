using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IRegistosProducao
{
    public interface IRegistosProducaoService
    {
        Task<IEnumerable<RegistosProducao>> GetAllRegistosProducaoAsync();
        Task<RegistosProducao?> GetRegistoProducaoByIdAsync(int id);
        Task<RegistosProducao> CreateRegistoProducaoAsync(RegistosProducao registo);
        Task UpdateRegistoProducaoAsync(RegistosProducao registo);
        Task DeleteRegistoProducaoAsync(int id);
        Task GetHistoricoAsync(int moldeId, int faseId, int pecaId, int operadorId);
    }
}
