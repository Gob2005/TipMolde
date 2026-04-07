using TipMolde.Core.Enums;
using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IEncomenda
{
    public interface IEncomendaRepository : IGenericRepository<Encomenda>
    {
        Task<Encomenda?> GetWithMoldesAsync(int id);
        Task<Encomenda?> GetByNumeroEncomendaClienteAsync(string numero);
        Task<IEnumerable<Encomenda>> GetByEstadoAsync(EstadoEncomenda estado);
        Task<IEnumerable<Encomenda>> GetEncomendasPorConcluirAsync();
    }
}
