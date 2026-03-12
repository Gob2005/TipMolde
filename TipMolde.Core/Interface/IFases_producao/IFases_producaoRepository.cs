using TipMolde.Core.Enums;
using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IFases_producao
{
    public interface IFases_producaoRepository : IGenericRepository<Fases_producao>
    {
        Task<Fases_producao?> GetByNomeAsync(Nome_fases nome);
    }
}
