using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Application.Interface.Desenho.IProjeto
{
    public interface IProjetoService
    {
        Task<PagedResult<Projeto>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<Projeto?> GetByIdAsync(int id);
        Task<Projeto?> GetWithRevisoesAsync(int id);
        Task<IEnumerable<Projeto>> GetByMoldeIdAsync(int moldeId);
        Task<Projeto> CreateAsync(Projeto projeto);
        Task UpdateAsync(Projeto projeto);
        Task DeleteAsync(int id);
    }
}
