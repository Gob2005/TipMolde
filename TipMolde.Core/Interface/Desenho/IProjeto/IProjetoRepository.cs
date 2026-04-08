using TipMolde.Core.Models.Desenho;

namespace TipMolde.Core.Interface.Desenho.IProjeto
{
    public interface IProjetoRepository : IGenericRepository<Projeto>
    {
        Task<IEnumerable<Projeto>> GetByMoldeIdAsync(int moldeId);
        Task<Projeto?> GetWithRevisoesAsync(int id);
    }
}
