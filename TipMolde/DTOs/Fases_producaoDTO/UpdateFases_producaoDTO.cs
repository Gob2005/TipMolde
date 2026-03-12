using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.Fases_producaoDTO
{
    public class UpdateFases_producaoDTO
    {
        public int Fases_producao_id { get; set; }
        public Nome_fases? Nome { get; set; }
        public string? Descricao { get; set; }
    }
}
