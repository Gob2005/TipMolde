using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Interface.Producao.IRegistosProducao
{
    public interface IRegistosProducaoService
    {
        Task<IEnumerable<RegistosProducao>> GetAllRegistosProducaoAsync();
        Task<RegistosProducao?> GetRegistoProducaoByIdAsync(int id);
        Task<RegistosProducao> CreateRegistoProducaoAsync(RegistosProducao registo);
        Task DeleteRegistoProducaoAsync(int id);
        Task<IEnumerable<RegistosProducao>> GetHistoricoAsync(int faseId, int pecaId);
        Task<RegistosProducao?> GetUltimoRegistoAsync(int faseId, int pecaId);
    }
}
