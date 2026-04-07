using TipMolde.Core.Enums;

namespace TipMolde.Core.Models
{
    public class Encomenda
    {
        public int Encomenda_id { get; set; }
        public required string NumeroEncomendaCliente { get; set; }
        public string? NumeroProjetoCliente { get; set; }
        public string? NomeServicoCliente { get; set; }
        public string? NomeResponsavelCliente { get; set; }
        public DateTime DataRegisto { get; set; } = DateTime.UtcNow;
        public EstadoEncomenda Estado { get; set; } = EstadoEncomenda.CONFIRMADA;

        public int Cliente_id { get; set; }
        public Cliente? Cliente { get; set; }

        public ICollection<EncomendaMolde> EncomendasMoldes { get; set; } = new List<EncomendaMolde>();
    }

}
