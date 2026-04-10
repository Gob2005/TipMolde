using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Application.Interface.Desenho.IRegistoTempoProjeto
{
    public interface IRegistoTempoProjetoService
    {
        Task<IEnumerable<RegistoTempoProjeto>> GetHistoricoAsync(int projetoId, int autorId);
        Task<RegistoTempoProjeto> CreateRegistoAsync(RegistoTempoProjeto registo);
        Task DeleteAsync(int id);
    }
}
