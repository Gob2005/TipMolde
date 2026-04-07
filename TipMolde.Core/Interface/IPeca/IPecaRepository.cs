using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IPeca
{
    public interface IPecaRepository : IGenericRepository<Peca>
    {
        Task<IEnumerable<Peca>> GetByMoldeIdAsync(int moldeId);
        Task<Peca?> GetByDesignacaoAsync(string designacao, int moldeId);
    }
}
