namespace TipMolde.Core.Models.Fichas
{
    public class RegistoMelhoriaAlteracao
    {
        public int RegistoMelhoriaAlteracao_id { get; set; }
        public DateTime DataRegisto { get; set; }
        public required string ItemDescricao { get; set; }
        public string? Pormenor { get; set; }
        public bool Verificado { get; set; }

        public int Responsavel_id { get; set; }
        public User? Responsavel { get; set; }

        public int FichaProducao_id { get; set; }
        public FichaProducao? FichaProducao { get; set; }
    }
}
