namespace TipMolde.Application.Dtos.RelatorioDto
{
    public sealed class MoldeProjetoResumoDto
    {
        public int ProjetoId { get; set; }
        public string NomeProjeto { get; set; } = string.Empty;
        public string SoftwareUtilizado { get; set; } = string.Empty;
        public string TipoProjeto { get; set; } = string.Empty;
    }
}
