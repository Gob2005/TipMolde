using TipMolde.Core.Enums;
using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IEncomenda
{
    public interface IEncomendaService
    {
        Task<IEnumerable<Encomenda>> GetAllEncomendasAsync();
        Task<Encomenda?> GetEncomendaByIdAsync(int id);
        Task<Encomenda?> GetEncomendaWithMoldesAsync(int id);
        Task<IEnumerable<Encomenda>> GetByEstadoAsync(EstadoEncomenda estado);
        Task<IEnumerable<Encomenda>> GetEncomendasPorConcluirAsync();
        Task<Encomenda?> GetByNumeroEncomendaClienteAsync(string numero);

        Task<Encomenda> CreateEncomendaAsync(Encomenda encomenda);
        Task UpdateEncomendaAsync(Encomenda encomenda);
        Task UpdateEstadoEncomendaAsync(int id, EstadoEncomenda novoEstado);
        Task DeleteEncomendaAsync(int id);
    }
}
