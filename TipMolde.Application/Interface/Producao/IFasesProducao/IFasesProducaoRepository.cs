using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Interface.Producao.IFasesProducao
{
    public interface IFasesProducaoRepository : IGenericRepository<FasesProducao, int>
    {
        Task<FasesProducao?> GetByNomeAsync(Nome_fases nome);
    }
}
