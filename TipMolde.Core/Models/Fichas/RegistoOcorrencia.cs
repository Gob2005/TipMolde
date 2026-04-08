namespace TipMolde.Core.Models.Fichas
{
    public class RegistoOcorrencia
    {
        public int RegistoOcorrencia_id { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public required string Descricao { get; set; }
        public string? Correcao { get; set; }

        public int Responsavel_id { get; set; }
        public User? Responsavel { get; set; }

        public int FichaProducao_id { get; set; }
        public FichaProducao? FichaProducao { get; set; }
    }
}
