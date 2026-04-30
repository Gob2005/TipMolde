namespace TipMolde.Application.Dtos.FichaDocumentoDto
{
    /// <summary>
    /// Representa os metadados seguros de um documento associado a uma ficha de producao.
    /// </summary>
    /// <remarks>
    /// Este DTO oculta detalhes internos de persistencia, como caminho fisico e hash,
    /// para manter o contrato HTTP desacoplado do modelo de armazenamento.
    /// </remarks>
    public class ResponseFichaDocumentoDto
    {
        public int FichaDocumento_id { get; set; }
        public int FichaProducao_id { get; set; }
        public int Versao { get; set; }
        public string Origem { get; set; } = string.Empty;
        public string NomeFicheiro { get; set; } = string.Empty;
        public string TipoFicheiro { get; set; } = string.Empty;
        public int CriadoPor_user_id { get; set; }
        public bool Ativo { get; set; }
    }
}
