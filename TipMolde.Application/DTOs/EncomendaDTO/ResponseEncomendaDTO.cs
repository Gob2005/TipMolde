using TipMolde.Domain.Enums;

namespace TipMolde.Application.DTOs.EncomendaDTO
{
    public class ResponseEncomendaDTO
    {
        public int Encomenda_id { get; set; }
        public string? NumeroEncomendaCliente { get; set; }
        public string? NumeroProjetoCliente { get; set; }
        public string? NomeServicoCliente { get; set; }
        public string? NomeResponsavelCliente { get; set; }
        public DateTime DataRegisto { get; set; }
        public EstadoEncomenda Estado { get; set; }
        public int Cliente_id { get; set; }
        public string? NomeCliente { get; set; }
    }
}
