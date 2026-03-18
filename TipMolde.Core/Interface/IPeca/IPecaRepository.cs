using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IPeca
{
    public interface IPecaRepository : IGenericRepository<Peca>
    {
        Task<Peca?> GetByNumberAsync(int peca_id, int molde_id);
    }
}
