namespace TipMolde.Core.Models.Desenho
{
    public class Revisao
    {
        public int Revisao_id { get; set; }
        public int NumRevisao { get; set; }
        public required string DescricaoAlteracoes { get; set; }
        public DateTime DataEnvioCliente { get; set; }
        public bool? Aprovado { get; set; }
        public DateTime? DataResposta { get; set; }
        public string? FeedbackTexto { get; set; }
        public string? FeedbackImagemPath { get; set; }

        public int Projeto_id { get; set; }
        public Projeto? Projeto { get; set; }
    }
}
