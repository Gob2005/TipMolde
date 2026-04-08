using TipMolde.Core.Enums;
using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Interface.Producao.IMaquina
{
    public interface IMaquinaService
    {
        Task<IEnumerable<Maquina>> GetAllAsync();
        Task<Maquina?> GetByIdAsync(int id);
        Task<IEnumerable<Maquina>> GetByEstadoAsync(EstadoMaquina estado);

        Task<Maquina> CreateAsync(Maquina maquina);
        Task UpdateAsync(Maquina maquina);
        Task DeleteAsync(int id);
    }
}
