namespace TipMolde.Application.Interface.Fichas.IFichaDocumento
{
    /// <summary>
    /// Abstrai o armazenamento fisico dos documentos associados as fichas de producao.
    /// </summary>
    public interface IFichaDocumentoStorage
    {
        /// <summary>
        /// Persiste o conteudo binario de um documento na area de armazenamento da ficha.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha dona do documento.</param>
        /// <param name="fileName">Nome final do ficheiro a persistir.</param>
        /// <param name="content">Conteudo binario do ficheiro.</param>
        /// <returns>Caminho fisico final onde o ficheiro ficou guardado.</returns>
        Task<string> SaveAsync(int fichaId, string fileName, byte[] content);

        /// <summary>
        /// Carrega o conteudo binario de um documento previamente persistido.
        /// </summary>
        /// <param name="path">Caminho fisico do ficheiro.</param>
        /// <returns>Conteudo binario do ficheiro.</returns>
        Task<byte[]> ReadAsync(string path);

        /// <summary>
        /// Verifica se o ficheiro existe no armazenamento fisico.
        /// </summary>
        /// <param name="path">Caminho fisico do ficheiro.</param>
        /// <returns>True quando o ficheiro existe.</returns>
        bool Exists(string path);

        /// <summary>
        /// Remove um ficheiro do armazenamento fisico quando ele existe.
        /// </summary>
        /// <param name="path">Caminho fisico do ficheiro.</param>
        Task DeleteIfExistsAsync(string? path);
    }
}
