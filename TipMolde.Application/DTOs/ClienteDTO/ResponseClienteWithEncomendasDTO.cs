using TipMolde.Application.DTOs.EncomendaDTO;

namespace TipMolde.Application.DTOs.ClienteDTO
{
    /// <summary>
    /// DTO de resposta com os dados do cliente e a colecao de encomendas associadas.
    /// </summary>
    /// <remarks>
    /// Adequado para cenarios de detalhe em que o consumidor precisa da relacao cliente-encomendas no mesmo payload.
    /// </remarks>
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
