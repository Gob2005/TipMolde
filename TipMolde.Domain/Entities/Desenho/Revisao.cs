namespace TipMolde.Domain.Entities.Desenho
{
    /// <summary>
    /// Representa uma revisao enviada ao cliente no contexto de um projeto de desenho.
    /// </summary>
    /// <remarks>
    /// Cada revisao pertence a um projeto e tem numeracao sequencial por projeto.
    /// Depois de a resposta do cliente ficar registada, a decisao nao deve ser sobrescrita
    /// para preservar rastreabilidade funcional e auditoria do processo.
    /// </remarks>
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
