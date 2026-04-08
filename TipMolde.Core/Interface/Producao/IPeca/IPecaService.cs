using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Interface.Producao.IPeca
{
    public interface IPecaService
    {
        Task<IEnumerable<Peca>> GetAllPecasAsync();
        Task<Peca?> GetPecaByIdAsync(int id);
        Task<IEnumerable<Peca>> GetByMoldeIdAsync(int moldeId);
        Task<Peca?> GetByDesignacaoAsync(string designacao, int moldeId);

        Task<Peca> CreatePecaAsync(Peca peca);
        Task UpdatePecaAsync(Peca peca);
        Task DeletePecaAsync(int id);

        //Task<int> ImportarCsvAsync(int moldeId, Stream csvStream);
    }
}
