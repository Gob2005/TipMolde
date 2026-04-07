using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.EncomendaDTO
{
    public class UpdateEncomendaDTO
    {
        public string? NumeroEncomendaCliente { get; set; }
        public string? NumeroProjetoCliente { get; set; }
        public string? NomeServicoCliente { get; set; }
        public string? NomeResponsavelCliente { get; set; }
    }
}
