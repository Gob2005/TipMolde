using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Interface.Producao.IMaquina
{
    public interface IMaquinaRepository : IGenericRepository<Maquina, int>
    {
        Task<Maquina?> GetByIdUnicoAsync(int id);
        Task<IEnumerable<Maquina>> GetByEstadoAsync(EstadoMaquina estado);
    }
}
