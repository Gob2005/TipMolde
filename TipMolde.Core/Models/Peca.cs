namespace TipMolde.Core.Models
{
    public class Peca
    {
        public int Peca_id { get; set; }
        public int Numero_peca { get; set; }
        public int Prioridade { get; set; }
        public string? Descricao { get; set; }

        public int Molde_id { get; set; }
        public Molde? Molde { get; set; }
    }
}
