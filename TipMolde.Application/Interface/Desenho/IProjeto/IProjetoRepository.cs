using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Application.Interface.Desenho.IProjeto
{
    public interface IProjetoRepository : IGenericRepository<Projeto, int>
    {
        Task<IEnumerable<Projeto>> GetByMoldeIdAsync(int moldeId);
        Task<Projeto?> GetWithRevisoesAsync(int id);
    }
}
