using TipMolde.Core.Models.Desenho;

namespace TipMolde.Core.Interface.Desenho.IRevisao
{
    public interface IRevisaoRepository : IGenericRepository<Revisao>
    {
        Task<IEnumerable<Revisao>> GetByProjetoIdAsync(int projetoId);
        Task<int> GetNextNumeroRevisaoAsync(int projetoId);
    }
}
