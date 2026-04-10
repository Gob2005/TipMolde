using TipMolde.Application.DTOs.EncomendaDTO;

namespace TipMolde.Application.DTOs.ClienteDTO
{
    public class ResponseClienteWithEncomendasDTO
    {
        public int ClienteId { get; set; }
        public string? Nome { get; set; }
        public string? Sigla { get; set; }
        public string? Pais { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? NIF { get; set; }
        public IEnumerable<ResponseEncomendaDTO>? Encomendas { get; set; }
    }
}
