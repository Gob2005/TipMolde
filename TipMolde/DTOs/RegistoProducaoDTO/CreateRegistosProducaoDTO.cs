using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.RegistoProducaoDTO
{
    public class CreateRegistosProducaoDTO
    {
        public int Molde_id { get; set; }
        public int Fase_id { get; set; }
        public int Operador_id { get; set; }
        public int Peca_id { get; set; }
        public string? Maquina { get; set; }
        public EstadoProducao Estado_producao { get; set; }
    }
}
