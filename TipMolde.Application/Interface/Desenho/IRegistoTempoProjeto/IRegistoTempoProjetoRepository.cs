using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Application.Interface.Desenho.IRegistoTempoProjeto
{
    public interface IRegistoTempoProjetoRepository : IGenericRepository<RegistoTempoProjeto, int>
    {
        Task<IEnumerable<RegistoTempoProjeto>> GetHistoricoAsync(int projetoId, int autorId);
        Task<RegistoTempoProjeto?> GetUltimoRegistoAsync(int projetoId, int autorId);
    }
}
