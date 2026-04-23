using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Producao.IPeca
{
    public interface IPecaService
    {
        Task<PagedResult<Peca>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<Peca?> GetByIdAsync(int id);
        Task<PagedResult<Peca>> GetByMoldeIdAsync(int moldeId, int page = 1, int pageSize = 10);
        Task<Peca?> GetByDesignacaoAsync(string designacao, int moldeId);

        Task<Peca> CreateAsync(Peca peca);
        Task UpdateAsync(Peca peca);
        Task DeleteAsync(int id);

        //Task<int> ImportarCsvAsync(int moldeId, Stream csvStream);
    }
}
