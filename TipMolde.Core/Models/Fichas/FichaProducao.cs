using TipMolde.Core.Enums;
using TipMolde.Core.Models.Comercio;

namespace TipMolde.Core.Models.Fichas
{
    public class FichaProducao
    {
        public int FichaProducao_id { get; set; }
        public TipoFicha Tipo { get; set; }
        public DateTime DataGeracao { get; set; }

        public int EncomendaMolde_id { get; set; }
        public EncomendaMolde? EncomendaMolde { get; set; }

        public ICollection<RegistoOcorrencia> RegistosOcorrencia { get; set; } = new List<RegistoOcorrencia>();
        public ICollection<RegistoMelhoriaAlteracao> RegistosMelhoriaAlteracao { get; set; } = new List<RegistoMelhoriaAlteracao>();
        public RegistoEnsaio? RegistoEnsaio { get; set; }
    }
}
