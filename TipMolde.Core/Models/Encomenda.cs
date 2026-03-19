using TipMolde.Core.Enums;

namespace TipMolde.Core.Models
{
    public class Encomenda
    {
        public int Encomenda_id { get; set; }
        public required string NumeroEncomendaCliente { get; set; }
        public EstadoEncomenda Estado { get; set; } = EstadoEncomenda.CONFIRMADA;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int Cliente_id { get; set; }
        public Cliente? Cliente { get; set; }

        public ICollection<Molde> Moldes { get; set; } = new List<Molde>();
    }
}
