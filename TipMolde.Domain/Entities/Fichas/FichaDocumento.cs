namespace TipMolde.Domain.Entities.Fichas
{
    /// <summary>
    /// Representa uma versao documental associada a uma ficha de producao.
    /// </summary>
    /// <remarks>
    /// A entidade guarda metadados de rastreabilidade, auditoria e integridade do artefacto
    /// oficial da ficha, incluindo versao, origem, utilizador criador e localizacao fisica.
    /// </remarks>
    public class FichaDocumento
    {
        public int FichaDocumento_id { get; set; }

        public int FichaProducao_id { get; set; }
        public FichaProducao? FichaProducao { get; set; }

        public int CriadoPor_user_id { get; set; }
        public User? CriadoPor { get; set; }

        public int Versao { get; set; }
        public string Origem { get; set; } = "SISTEMA";
        public string NomeFicheiro { get; set; } = string.Empty;
        public string TipoFicheiro { get; set; } = string.Empty;
        public string CaminhoFicheiro { get; set; } = string.Empty;
        public string? HashSha256 { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
