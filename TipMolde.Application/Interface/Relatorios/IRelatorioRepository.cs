using TipMolde.Application.Dtos.RelatorioDto;

namespace TipMolde.Application.Interface.Relatorios
{
    /// <summary>
    /// Define queries especializadas para relatorios e indicadores.
    /// </summary>
    /// <remarks>
    /// O repositorio devolve read-models dedicados para evitar acoplamento do formato
    /// do documento ao modelo EF Core usado na persistencia.
    /// </remarks>
    public interface IRelatorioRepository
    {
        /// <summary>
        /// Obtem os dados agregados do ciclo de vida de um molde.
        /// </summary>
        /// <param name="moldeId">Identificador interno do molde.</param>
        /// <returns>Read-model do relatorio ou nulo quando o molde nao existe.</returns>
        Task<MoldeCicloVidaRelatorioDto?> ObterMoldeCicloVidaAsync(int moldeId);

        /// <summary>
        /// Obtem o contexto base usado pelas fichas de producao exportadas.
        /// </summary>
        /// <param name="fichaId">Identificador interno da ficha.</param>
        /// <returns>Read-model base da ficha ou nulo quando a ficha nao existe.</returns>
        Task<FichaRelatorioBaseDto?> ObterFichaRelatorioBaseAsync(int fichaId);
    }
}
