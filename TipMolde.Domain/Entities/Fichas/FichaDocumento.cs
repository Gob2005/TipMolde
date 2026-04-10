namespace TipMolde.Domain.Entities.Fichas
{
    public class FichaDocumento
    {
        public int FichaDocumento_id { get; set; }

        public int FichaProducao_id { get; set; }
        public FichaProducao? FichaProducao { get; set; }

        public int CriadoPor_user_id { get; set; }
        public User CriadoPor { get; set; }

        public int Versao { get; set; }
        public string Origem { get; set; } = "SISTEMA"; // SISTEMA | UPLOAD
        public string NomeFicheiro { get; set; } = string.Empty;
        public string TipoFicheiro { get; set; } = "";
        public string CaminhoFicheiro { get; set; } = "";
        public string? HashSha256 { get; set; }         // integridade (ISO)
        public bool Ativo { get; set; } = true;
    }
}
