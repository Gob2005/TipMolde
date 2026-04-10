using TipMolde.Domain.Enums;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.IEncomenda
{
    public interface IEncomendaService
    {
        Task<PagedResult<Encomenda>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<Encomenda?> GetByIdAsync(int id);
        Task<Encomenda?> GetEncomendaWithMoldesAsync(int id);
        Task<IEnumerable<Encomenda>> GetByEstadoAsync(EstadoEncomenda estado);
        Task<IEnumerable<Encomenda>> GetEncomendasPorConcluirAsync();
        Task<Encomenda?> GetByNumeroEncomendaClienteAsync(string numero);

        Task<Encomenda> CreateAsync(Encomenda encomenda);
        Task UpdateAsync(Encomenda encomenda);
        Task UpdateEstadoAsync(int id, EstadoEncomenda novoEstado);
        Task DeleteAsync(int id);
    }
}
