using TipMolde.Application.Dtos.FichaDocumentoDto;

namespace TipMolde.Application.Interface.Fichas.IFichaDocumento
{
    /// <summary>
    /// Define os casos de uso de versionamento, upload e consulta documental das fichas.
    /// </summary>
    public interface IFichaDocumentoService
    {
        /// <summary>
        /// Persiste um documento gerado automaticamente pelo sistema.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha de producao dona do documento.</param>
        /// <param name="content">Conteudo binario do ficheiro gerado.</param>
        /// <param name="fileName">Nome base do ficheiro antes da versao final ser aplicada.</param>
        /// <param name="tipoFicheiro">Content type do ficheiro a persistir.</param>
        /// <param name="userId">Utilizador responsavel pela geracao.</param>
        /// <param name="origem">Origem funcional do documento.</param>
        /// <returns>DTO seguro com os metadados da nova versao criada.</returns>
        Task<ResponseFichaDocumentoDto> GuardarGeradoAsync(int fichaId, byte[] content, string fileName, string tipoFicheiro, int userId, string origem);

        /// <summary>
        /// Persiste um documento enviado manualmente por um utilizador.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha de producao dona do documento.</param>
        /// <param name="dto">Input normalizado do upload recebido no boundary HTTP.</param>
        /// <param name="userId">Identificador do utilizador autenticado.</param>
        /// <returns>DTO seguro com os metadados da nova versao criada.</returns>
        Task<ResponseFichaDocumentoDto> UploadAsync(int fichaId, UploadFichaDocumentoDto dto, int userId);

        /// <summary>
        /// Lista as versoes documentais associadas a uma ficha de forma paginada.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha de producao.</param>
        /// <param name="page">Pagina pedida pelo consumidor.</param>
        /// <param name="pageSize">Quantidade maxima de registos por pagina.</param>
        /// <returns>Pagina com as versoes documentais da ficha.</returns>
        Task<PagedResult<ResponseFichaDocumentoDto>> ListarAsync(int fichaId, int page = 1, int pageSize = 10);

        /// <summary>
        /// Carrega o conteudo de um documento persistido pertencente a uma ficha especifica.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha.</param>
        /// <param name="documentoId">Identificador do documento.</param>
        /// <returns>Conteudo binario, nome final e tipo MIME do ficheiro.</returns>
        Task<FichaDocumentoDownloadResultDto> DownloadAsync(int fichaId, int documentoId);
    }
}
