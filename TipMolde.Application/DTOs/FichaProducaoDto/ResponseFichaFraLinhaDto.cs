namespace TipMolde.Application.Dtos.FichaProducaoDto
{
    /// <summary>
    /// Representa uma linha publica da ficha FRA.
    /// </summary>
    public class ResponseFichaFraLinhaDto
    {
        public int FichaFraLinha_id { get; set; }
        public int FichaFra_id { get; set; }
        public DateTime Data { get; set; }
        public string Alteracoes { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public int Responsavel_id { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
