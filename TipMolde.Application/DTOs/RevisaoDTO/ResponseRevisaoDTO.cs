namespace TipMolde.Application.DTOs.RevisaoDTO
{
    public class ResponseRevisaoDTO
    {
        public int Revisao_id { get; set; }
        public int NumRevisao { get; set; }
        public string DescricaoAlteracoes { get; set; } = string.Empty;
        public DateTime DataEnvioCliente { get; set; }
        public bool? Aprovado { get; set; }
        public DateTime? DataResposta { get; set; }
        public string? FeedbackTexto { get; set; }
        public string? FeedbackImagemPath { get; set; }
        public int Projeto_id { get; set; }
    }
}
