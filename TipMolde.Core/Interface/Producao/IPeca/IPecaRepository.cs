using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Interface.Producao.IPeca
{
    public interface IPecaRepository : IGenericRepository<Peca>
    {
        Task<IEnumerable<Peca>> GetByMoldeIdAsync(int moldeId);
        Task<Peca?> GetByDesignacaoAsync(string designacao, int moldeId);
    }
}
