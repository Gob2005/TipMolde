using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.FichaProducaoDto
{
    public class ResponseFichaProducaoDto
    {
        public int FichaProducao_id { get; set; }
        public TipoFicha Tipo { get; set; }
        public DateTime DataGeracao { get; set; }
        public int EncomendaMolde_id { get; set; }
    }
}
