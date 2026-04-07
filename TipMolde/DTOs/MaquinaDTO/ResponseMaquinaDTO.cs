using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.MaquinaDTO
{
    public class ResponseMaquinaDTO
    {
        public int Maquina_id { get; set; }
        public string? NomeModelo { get; set; }
        public string? IpAddress { get; set; }
        public EstadoMaquina Estado { get; set; }
    }
}
