namespace TipMolde.Application.Dtos.FichaDocumentoDto
{
    /// <summary>
    /// Representa o input normalizado de upload manual de um documento de ficha.
    /// </summary>
    public class UploadFichaDocumentoDto
    {
        public byte[] Content { get; set; } = [];
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
