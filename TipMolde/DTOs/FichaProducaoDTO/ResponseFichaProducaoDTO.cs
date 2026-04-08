using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.FichaProducaoDTO
{
    public class ResponseFichaProducaoDTO
    {
        public int FichaProducao_id { get; set; }
        public TipoFicha Tipo { get; set; }
        public DateTime DataGeracao { get; set; }
        public int EncomendaMolde_id { get; set; }
    }
}
