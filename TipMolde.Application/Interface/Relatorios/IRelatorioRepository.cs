using TipMolde.Domain.Entities.Fichas;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Relatorios
{
    public interface IRelatorioRepository
    {
        Task<Molde?> GetMoldeComEspecificacoesAsync(int moldeId);
        Task<FichaProducao?> GetFichaFltCompletaAsync(int fichaId);
    }
}
