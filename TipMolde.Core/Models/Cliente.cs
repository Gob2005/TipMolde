namespace TipMolde.Core.Models
{
    public class Cliente
    {
        public int Cliente_id { get; set; }
        public string? Pais { get; set; }
        public required string Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public required string NIF { get; set; }
        public required string Sigla { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Encomenda> Encomendas { get; set; } = new List<Encomenda>();

    }
}
