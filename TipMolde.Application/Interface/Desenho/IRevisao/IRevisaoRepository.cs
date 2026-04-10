using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Application.Interface.Desenho.IRevisao
{
    public interface IRevisaoRepository : IGenericRepository<Revisao, int>
    {
        Task<IEnumerable<Revisao>> GetByProjetoIdAsync(int projetoId);
        Task<int> GetNextNumeroRevisaoAsync(int projetoId);
    }
}
