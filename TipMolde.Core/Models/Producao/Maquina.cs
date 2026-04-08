using TipMolde.Core.Enums;

namespace TipMolde.Core.Models.Producao
{
    public class Maquina
    {
        public int Maquina_id { get; set; }
        public required string NomeModelo { get; set; }
        public string? IpAddress { get; set; }
        public required EstadoMaquina Estado { get; set; } = EstadoMaquina.DISPONIVEL;
    }

}
