using TipMolde.Core.Enums;
using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Interface.Producao.IFasesProducao
{
    public interface IFasesProducaoRepository : IGenericRepository<FasesProducao>
    {
        Task<FasesProducao?> GetByNomeAsync(Nome_fases nome);
    }
}
