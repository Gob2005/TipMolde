using TipMolde.Core.Enums;
using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Interface.Producao.IMaquina
{
    public interface IMaquinaRepository : IGenericRepository<Maquina>
    {
        Task<Maquina?> GetByIdUnicoAsync(int id);
        Task<IEnumerable<Maquina>> GetByEstadoAsync(EstadoMaquina estado);
    }
}
