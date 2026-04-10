using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Producao.IPeca
{
    public interface IPecaRepository : IGenericRepository<Peca, int>
    {
        Task<IEnumerable<Peca>> GetByMoldeIdAsync(int moldeId);
        Task<Peca?> GetByDesignacaoAsync(string designacao, int moldeId);
    }
}
