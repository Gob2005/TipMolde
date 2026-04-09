using TipMolde.Core.Models.Fichas;
using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Interface.Relatorios
{
    public interface IRelatorioRepository
    {
        Task<Molde?> GetMoldeComEspecificacoesAsync(int moldeId);
        Task<FichaProducao?> GetFichaFltCompletaAsync(int fichaId);
    }
}
