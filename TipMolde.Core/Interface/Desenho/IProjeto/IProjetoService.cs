using TipMolde.Core.Models.Desenho;

namespace TipMolde.Core.Interface.Desenho.IProjeto
{
    public interface IProjetoService
    {
        Task<IEnumerable<Projeto>> GetAllAsync();
        Task<Projeto?> GetByIdAsync(int id);
        Task<Projeto?> GetWithRevisoesAsync(int id);
        Task<IEnumerable<Projeto>> GetByMoldeIdAsync(int moldeId);
        Task<Projeto> CreateAsync(Projeto projeto);
        Task UpdateAsync(Projeto projeto);
        Task DeleteAsync(int id);
    }
}
