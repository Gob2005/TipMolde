namespace TipMolde.Application.DTOs.PecaDTO
{
    /// <summary>
    /// Representa a resposta publica da feature Peca.
    /// </summary>
    public class ResponsePecaDTO
    {
        public int PecaId { get; set; }
        public string? Designacao { get; set; }
        public int Prioridade { get; set; }
        public string? MaterialDesignacao { get; set; }
        public bool MaterialRecebido { get; set; }
        public int Molde_id { get; set; }
    }
}
