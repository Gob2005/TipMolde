namespace TipMolde.Core.Models.Desenho
{
    public class RegistoTempoProjeto
    {
        public int RegistoTempoProjeto_id { get; set; }
        public DateTime DataHora { get; set; }
        public int Duracao { get; set; }
        public string? Descricao { get; set; }
        public string? EstadoTrabalho { get; set; }

        public int Projeto_id { get; set; }
        public Projeto? Projeto { get; set; }

        public int Autor_id { get; set; }
        public User? Autor { get; set; }
    }
}
