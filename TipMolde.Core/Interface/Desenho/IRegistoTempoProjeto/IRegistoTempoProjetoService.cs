using TipMolde.Core.Models.Desenho;

namespace TipMolde.Core.Interface.Desenho.IRegistoTempoProjeto
{
    public interface IRegistoTempoProjetoService
    {
        Task<IEnumerable<RegistoTempoProjeto>> GetHistoricoAsync(int projetoId, int autorId);
        Task<RegistoTempoProjeto> CreateRegistoAsync(RegistoTempoProjeto registo);
        Task DeleteAsync(int id);
    }
}
