namespace TipMolde.Core.Models
{
    public class Fornecedor
    {
        public int Fornecedor_id { get; set; }
        public required string Nome { get; set; }
        public required string NIF { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public required string Morada { get; set; }
    }

}
