using TipMolde.Domain.Enums;

namespace TipMolde.Application.DTOs.MaquinaDTO
{
    public class ResponseMaquinaDTO
    {
        public int Maquina_id { get; set; }
        public string? NomeModelo { get; set; }
        public string? IpAddress { get; set; }
        public EstadoMaquina Estado { get; set; }
    }
}
