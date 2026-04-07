namespace TipMolde.API.DTOs.ClienteDTO
{
    public class ResponseClienteDTO
    {
        public int ClienteId { get; set; }
        public string? Nome { get; set; }
        public string? NIF { get; set; }
        public string? Sigla { get; set; }
        public string? Pais { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
    }
}

