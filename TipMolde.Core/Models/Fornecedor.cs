namespace TipMolde.Core.Models
{
    public class Fornecedor
    {
        int Fornecedor_id { get; set; }
        public required string Nome { get; set; }
        public string? Nome_responsavel { get; set; }
        public string? NIF { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
