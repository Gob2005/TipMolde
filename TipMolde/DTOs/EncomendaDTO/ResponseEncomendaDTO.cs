using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.EncomendaDTO
{
    public class ResponseEncomendaDTO
    {
        public int Encomenda_id { get; set; }
        public int Cliente_id { get; set; }
        public string? NomeCliente { get; set; }
        public required string NumeroEncomendaCliente { get; set; }
        public EstadoEncomenda Estado { get; set; }
    }
}
