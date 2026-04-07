namespace TipMolde.API.DTOs.EncomendaMoldeDTO
{
    public class CreateEncomendaMoldeDTO
    {
        public int Encomenda_id { get; set; }
        public int Molde_id { get; set; }
        public int Quantidade { get; set; }
        public int Prioridade { get; set; }
        public DateTime DataEntregaPrevista { get; set; }
    }
}
