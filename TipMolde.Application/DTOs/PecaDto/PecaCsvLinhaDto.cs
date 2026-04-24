namespace TipMolde.Application.Dtos.PecaDto
{
    /// <summary>
    /// Representa uma linha normalizada do CSV de pecas.
    /// </summary>
    /// <remarks>
    /// Este DTO e usado no parsing tecnico do ficheiro antes da consolidacao por NumeroPeca.
    /// </remarks>
    public class PecaCsvLinhaDto
    {
        public int NumeroLinha { get; set; }
        public string NumeroPeca { get; set; } = string.Empty;
        public string Designacao { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public string? Referencia { get; set; }
        public string? MaterialDesignacao { get; set; }
        public string? TratamentoTermico { get; set; }
        public string? Massa { get; set; }
        public string? Observacao { get; set; }
    }
}
