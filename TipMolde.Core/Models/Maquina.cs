using TipMolde.Core.Enums;

namespace TipMolde.Core.Models
{
    public class Maquina
    {
        public string Maquina_id { get; set; } = string.Empty;
        public required string NomeModelo { get; set; }
        public string? IpAddress { get; set; }
        public required EstadoMaquina Estado { get; set; }
    }

}
