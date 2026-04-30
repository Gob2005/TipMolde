namespace TipMolde.Application.Dtos.FichaDocumentoDto
{
    /// <summary>
    /// Representa os dados normalizados usados para criar uma nova versao documental.
    /// </summary>
    public class CreateFichaDocumentoDto
    {
        public int FichaProducao_id { get; set; }
        public int CriadoPor_user_id { get; set; }
        public int Versao { get; set; }
        public string Origem { get; set; } = string.Empty;
        public string NomeFicheiro { get; set; } = string.Empty;
        public string TipoFicheiro { get; set; } = string.Empty;
        public string CaminhoFicheiro { get; set; } = string.Empty;
        public string? HashSha256 { get; set; }
        public bool Ativo { get; set; }
    }
}
