using TipMolde.Core.Enums;
using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IFases_producao
{
    public interface IFasesProducaoRepository : IGenericRepository<FasesProducao>
    {
        Task<FasesProducao?> GetByNomeAsync(Nome_fases nome);
    }
}
