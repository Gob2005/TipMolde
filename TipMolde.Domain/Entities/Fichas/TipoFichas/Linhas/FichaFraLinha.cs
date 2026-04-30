namespace TipMolde.Domain.Entities.Fichas.TipoFichas.Linhas
{
    /// <summary>
    /// Representa uma linha manual da ficha FRA.
    /// </summary>
    public class FichaFraLinha
    {
        public int FichaFraLinha_id { get; set; }
        public int FichaFra_id { get; set; }
        public FichaFra? FichaFra { get; set; }
        public DateTime Data { get; set; }
        public string Alteracoes { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public int Responsavel_id { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
