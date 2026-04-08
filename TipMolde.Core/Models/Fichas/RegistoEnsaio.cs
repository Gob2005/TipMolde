using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Models.Fichas
{
    public class RegistoEnsaio
    {
        public int RegistoEnsaio_id { get; set; }
        public DateTime DataEnsaio { get; set; }
        public required string LocalEnsaio { get; set; }
        public bool AguasCavidade { get; set; }
        public bool AguasMacho { get; set; }
        public bool AguasMovimentos { get; set; }
        public string? ResumoTexto { get; set; }

        public int Maquina_id { get; set; }
        public Maquina? Maquina { get; set; }

        public int FichaProducao_id { get; set; }
        public FichaProducao? FichaProducao { get; set; }

        public int Responsavel_id { get; set; }
        public User? Responsavel { get; set; }
    }
}
