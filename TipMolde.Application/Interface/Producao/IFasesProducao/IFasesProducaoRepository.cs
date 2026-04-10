using TipMolde.Domain.Enums;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Producao.IFasesProducao
{
    public interface IFasesProducaoRepository : IGenericRepository<FasesProducao, int>
    {
        Task<FasesProducao?> GetByNomeAsync(Nome_fases nome);
    }
}
