using TipMolde.Core.Models.Desenho;

namespace TipMolde.Core.Interface.Desenho.IRegistoTempoProjeto
{
    public interface IRegistoTempoProjetoRepository : IGenericRepository<RegistoTempoProjeto>
    {
        Task<IEnumerable<RegistoTempoProjeto>> GetByProjetoIdAsync(int projetoId);
    }
}
