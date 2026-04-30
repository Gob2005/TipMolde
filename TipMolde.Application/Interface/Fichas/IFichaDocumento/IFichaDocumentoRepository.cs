using TipMolde.Domain.Entities.Fichas;

namespace TipMolde.Application.Interface.Fichas.IFichaDocumento
{
    /// <summary>
    /// Define as operacoes de persistencia e consulta dos documentos das fichas de producao.
    /// </summary>
    public interface IFichaDocumentoRepository
    {
        /// <summary>
        /// Verifica se a ficha de producao existe.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha.</param>
        /// <returns>True quando a ficha existe.</returns>
        Task<bool> FichaExisteAsync(int fichaId);

        /// <summary>
        /// Calcula a proxima versao documental da ficha.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha.</param>
        /// <returns>Numero sequencial da proxima versao.</returns>
        Task<int> GetProximaVersaoAsync(int fichaId);

        /// <summary>
        /// Desativa todas as versoes atualmente ativas da ficha.
        /// </summary>
        /// <remarks>
        /// Esta operacao deve ser executada no mesmo boundary transacional da criacao da nova versao.
        /// </remarks>
        /// <param name="fichaId">Identificador da ficha.</param>
        Task DesativarVersoesAtivasAsync(int fichaId);

        /// <summary>
        /// Persiste um novo documento da ficha.
        /// </summary>
        /// <param name="doc">Entidade documental a persistir.</param>
        Task AddAsync(FichaDocumento doc);

        /// <summary>
        /// Obtem um documento pelo identificador interno.
        /// </summary>
        /// <param name="id">Identificador do documento.</param>
        /// <returns>Documento encontrado ou nulo.</returns>
        Task<FichaDocumento?> GetByIdAsync(int id);

        /// <summary>
        /// Obtem um documento garantindo que pertence a uma ficha especifica.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha.</param>
        /// <param name="documentoId">Identificador do documento.</param>
        /// <returns>Documento encontrado ou nulo.</returns>
        Task<FichaDocumento?> GetByIdAndFichaIdAsync(int fichaId, int documentoId);

        /// <summary>
        /// Obtem a versao ativa mais recente de uma ficha.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha.</param>
        /// <returns>Documento ativo ou nulo.</returns>
        Task<FichaDocumento?> GetAtivoByFichaIdAsync(int fichaId);

        /// <summary>
        /// Lista todas as versoes documentais de uma ficha.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha.</param>
        /// <param name="page">Pagina pedida pelo consumidor.</param>
        /// <param name="pageSize">Quantidade maxima de registos por pagina.</param>
        /// <returns>Pagina com as versoes documentais da ficha.</returns>
        Task<PagedResult<FichaDocumento>> GetByFichaIdAsync(int fichaId, int page, int pageSize);
    }
}
