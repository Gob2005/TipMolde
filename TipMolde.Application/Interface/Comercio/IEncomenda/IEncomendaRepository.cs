using TipMolde.Domain.Enums;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.IEncomenda
{
    public interface IEncomendaRepository : IGenericRepository<Encomenda, int>
    {
        Task<Encomenda?> GetWithMoldesAsync(int id);
        Task<Encomenda?> GetByNumeroEncomendaClienteAsync(string numero);
        Task<IEnumerable<Encomenda>> GetByEstadoAsync(EstadoEncomenda estado);
        Task<IEnumerable<Encomenda>> GetEncomendasPorConcluirAsync();
    }
}
