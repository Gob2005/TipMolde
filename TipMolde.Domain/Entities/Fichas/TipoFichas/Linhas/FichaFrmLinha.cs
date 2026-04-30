namespace TipMolde.Domain.Entities.Fichas.TipoFichas.Linhas
{
    /// <summary>
    /// Representa uma linha manual da ficha FRM.
    /// </summary>
    public class FichaFrmLinha
    {
        public int FichaFrmLinha_id { get; set; }
        public int FichaFrm_id { get; set; }
        public FichaFrm? FichaFrm { get; set; }
        public DateTime Data { get; set; }
        public string Defeito { get; set; } = string.Empty;
        public string? Pormenor { get; set; }
        public bool Verificado { get; set; }
        public int Responsavel_id { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
