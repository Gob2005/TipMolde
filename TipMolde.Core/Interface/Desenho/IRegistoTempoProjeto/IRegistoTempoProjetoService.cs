using TipMolde.Core.Models.Desenho;

namespace TipMolde.Core.Interface.Desenho.IRegistoTempoProjeto
{
    public interface IRegistoTempoProjetoService
    {
        Task<IEnumerable<RegistoTempoProjeto>> GetByProjetoIdAsync(int projetoId);
        Task<RegistoTempoProjeto> CreateAsync(RegistoTempoProjeto registo);
        Task DeleteAsync(int id);
    }
}
