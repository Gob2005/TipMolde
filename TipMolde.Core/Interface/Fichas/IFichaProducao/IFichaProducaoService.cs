using TipMolde.Core.Models.Fichas;

namespace TipMolde.Core.Interface.Fichas.IFichaProducao
{
    public interface IFichaProducaoService
    {
        Task<IEnumerable<FichaProducao>> GetByEncomendaMoldeIdAsync(int encomendaMoldeId);
        Task<FichaProducao?> GetByIdWithHeaderAsync(int id);
        Task<FichaProducao?> GetFLTByIdAsync(int id);

        Task<FichaProducao> CreateAsync(FichaProducao ficha);
        Task<RegistoOcorrencia> AddOcorrenciaAsync(int fichaId, RegistoOcorrencia ocorrencia);
        Task<RegistoMelhoriaAlteracao> AddMelhoriaAlteracaoAsync(int fichaId, RegistoMelhoriaAlteracao registo);
        Task<RegistoEnsaio> UpsertEnsaioAsync(int fichaId, RegistoEnsaio ensaio);

        Task DeleteAsync(int id);
    }
}
