namespace TipMolde.Application.Dtos.FornecedorDto
{
    /// <summary>
    /// DTO de resposta com os dados resumidos de um fornecedor.
    /// </summary>
    /// <remarks>
    /// Usado em listagens e consultas simples onde nao e necessario devolver relacionamentos associados.
    /// </remarks>
    public class ResponseFornecedorDto
    {
        public int FornecedorId { get; set; }
        public string? Nome { get; set; }
        public string? NIF { get; set; }
        public string? Morada { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
    }
}