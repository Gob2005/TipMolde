namespace TipMolde.API.DTOs.EncomendaMoldeDTO
{
    public class UpdateEncomendaMoldeDTO
    {
        public int? Quantidade { get; set; }
        public int? Prioridade { get; set; }
        public DateTime? DataEntregaPrevista { get; set; }
    }
}

