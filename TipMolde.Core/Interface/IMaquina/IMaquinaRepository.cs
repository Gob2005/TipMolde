using TipMolde.Core.Enums;
using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IMaquina
{
    public interface IMaquinaRepository : IGenericRepository<Maquina>
    {
        Task<Maquina?> GetByIdUnicoAsync(int id);
        Task<IEnumerable<Maquina>> GetByEstadoAsync(EstadoMaquina estado);
    }
}
