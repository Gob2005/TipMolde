namespace TipMolde.Core.Models
{
    public class Cliente
    {
        public int Cliente_id { get; set; }
        public string Pais { get; set; }
        public string? Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string NIF { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
