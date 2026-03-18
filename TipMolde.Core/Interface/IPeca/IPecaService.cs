using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IPeca
{
    public interface IPecaService
    {
        Task<IEnumerable<Peca>> GetAllPecasAsync();
        Task<Peca?> GetPecaByIdAsync(int id);
        Task<Peca?> GetPecaByNumberAsync(int number);
        Task<Peca> CreatePecaAsync(Peca peca);
        Task UpdatePecaAsync(Peca peca);
        Task DeletePecaAsync(int id);
    }
}
