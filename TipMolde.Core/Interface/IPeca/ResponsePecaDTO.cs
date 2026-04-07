namespace TipMolde.API.DTOs.PecaDTO
{
    public class ResponsePecaDTO
    {
        public int PecaId { get; set; }
        public string? Designacao { get; set; }
        public int Prioridade { get; set; }
        public string? MaterialDesignacao { get; set; }
        public bool MaterialRecebido { get; set; }
        public int MoldeId { get; set; }
    }
}

