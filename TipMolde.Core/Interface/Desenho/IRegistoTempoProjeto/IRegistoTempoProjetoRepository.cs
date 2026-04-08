using TipMolde.Core.Models.Desenho;

namespace TipMolde.Core.Interface.Desenho.IRegistoTempoProjeto
{
    public interface IRegistoTempoProjetoRepository : IGenericRepository<RegistoTempoProjeto>
    {
        Task<IEnumerable<RegistoTempoProjeto>> GetHistoricoAsync(int projetoId, int autorId);
        Task<RegistoTempoProjeto?> GetUltimoRegistoAsync(int projetoId, int autorId);
    }
}
