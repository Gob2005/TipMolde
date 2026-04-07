namespace TipMolde.Core.Models
{
    public class EncomendaMolde
    {
        public int EncomendaMolde_id { get; set; }
        public int Quantidade { get; set; }
        public int Prioridade { get; set; }
        public DateTime DataEntregaPrevista { get; set; }

        public int Encomenda_id { get; set; }
        public Encomenda? Encomenda { get; set; }

        public int Molde_id { get; set; }
        public Molde? Molde { get; set; }

        public ICollection<FichaProducao> Fichas { get; set; } = new List<FichaProducao>();
    }

}
