namespace TipMolde.Application.Dtos.ClienteDto
{
    /// <summary>
    /// DTO de resposta com os dados resumidos de um cliente.
    /// </summary>
    /// <remarks>
    /// Usado em listagens e consultas simples onde nao e necessario devolver relacionamentos associados.
    /// </remarks>
    public class ResponseClienteDto
    {
        public int Cliente_id { get; set; }
        public string? Nome { get; set; }
        public string? NIF { get; set; }
        public string? Sigla { get; set; }
        public string? Pais { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
    }
}

