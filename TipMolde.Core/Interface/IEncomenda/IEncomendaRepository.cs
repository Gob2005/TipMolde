using TipMolde.Core.Enums;
using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IEncomenda
{
    public interface IEncomendaRepository : IGenericRepository<Encomenda>
    {
        Task<IEnumerable<Encomenda>> GetByEstadoAsync(EstadoEncomenda estado);
        Task<IEnumerable<Encomenda>> GetEncomendasPorConcluirAsync();
        Task<Encomenda?> GetByNumeroEncomendaClienteAsync(string numeroEncomendaCliente);
        Task<Encomenda?> GetWithMoldesAsync(int id);
    }
}
