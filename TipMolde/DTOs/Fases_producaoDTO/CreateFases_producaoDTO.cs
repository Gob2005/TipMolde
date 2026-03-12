using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.Fases_producaoDTO
{
    public class CreateFases_producaoDTO
    {
        public required Nome_fases Nome { get; set; }
        public string? Descricao { get; set; }
    }
}
