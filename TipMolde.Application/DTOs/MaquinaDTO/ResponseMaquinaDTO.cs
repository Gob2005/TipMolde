using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.MaquinaDto
{
    public class ResponseMaquinaDto
    {
        public int Maquina_id { get; set; }
        public string? NomeModelo { get; set; }
        public string? IpAddress { get; set; }
        public EstadoMaquina Estado { get; set; }
    }
}
