using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;

namespace TipMolde.Domain.Entities.Fichas
{
    public class FichaProducao
    {
        public int FichaProducao_id { get; set; }
        public TipoFicha Tipo { get; set; }
        public DateTime DataCriacao { get; set; }

        public int EncomendaMolde_id { get; set; }
        public EncomendaMolde? EncomendaMolde { get; set; }

        public ICollection<FichaDocumento> Relatorios { get; set; } = new List<FichaDocumento>();
    }
}
