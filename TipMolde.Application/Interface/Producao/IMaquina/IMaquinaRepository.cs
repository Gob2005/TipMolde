using TipMolde.Domain.Enums;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Producao.IMaquina
{
    public interface IMaquinaRepository : IGenericRepository<Maquina, int>
    {
        Task<Maquina?> GetByIdUnicoAsync(int id);
        Task<IEnumerable<Maquina>> GetByEstadoAsync(EstadoMaquina estado);
    }
}
