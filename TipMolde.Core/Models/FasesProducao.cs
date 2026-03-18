using TipMolde.Core.Enums;

namespace TipMolde.Core.Models
{
    public class FasesProducao
    {
        public int Fases_producao_id { get; set; }
        public required Nome_fases Nome { get; set; }
        public string? Descricao { get; set; }
    }
}
