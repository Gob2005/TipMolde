namespace TipMolde.Application.Dtos.FichaDocumentoDto
{
    /// <summary>
    /// Representa o resultado interno necessario para descarregar um documento da ficha.
    /// </summary>
    public class FichaDocumentoDownloadResultDto
    {
        public byte[] Content { get; set; } = [];
        public string FileName { get; set; } = string.Empty;
        public string TipoFicheiro { get; set; } = "application/octet-stream";
    }
}
