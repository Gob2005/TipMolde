using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Interface.Producao.IRegistosProducao
{
    public interface IRegistosProducaoRepository : IGenericRepository<RegistosProducao> 
    {
        Task<IEnumerable<RegistosProducao>> GetHistoricoAsync(int faseId, int pecaId);
        Task<RegistosProducao?> GetUltimoRegistoAsync(int faseId, int pecaId);
        Task<IEnumerable<RegistosProducao>> GetByMaquinaAsync(int maquinaId);
    }
}
