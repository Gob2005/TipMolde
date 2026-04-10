using TipMolde.Domain.Enums;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Producao.IMaquina
{
    public interface IMaquinaService
    {
        Task<PagedResult<Maquina>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<Maquina?> GetByIdAsync(int id);
        Task<IEnumerable<Maquina>> GetByEstadoAsync(EstadoMaquina estado);

        Task<Maquina> CreateAsync(Maquina maquina);
        Task UpdateAsync(Maquina maquina);
        Task DeleteAsync(int id);
    }
}
