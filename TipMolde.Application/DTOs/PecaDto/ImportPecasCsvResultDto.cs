namespace TipMolde.Application.Dtos.PecaDto
{
    /// <summary>
    /// Representa o resultado da importacao de pecas a partir de um CSV.
    /// </summary>
    /// <remarks>
    /// O resultado devolve contadores de processamento e a colecao final de pecas
    /// persistidas apos consolidacao por NumeroPeca.
    /// </remarks>
    public class ImportPecasCsvResultDto
    {
        public int MoldeId { get; set; }
        public string? ReferenciaMolde { get; set; }
        public string? MassaMolde { get; set; }
        public int TotalLinhasPecaLidas { get; set; }
        public int TotalPecasConsolidadas { get; set; }
        public int TotalQuantidadeConsolidada { get; set; }
        public List<ResponsePecaDto> PecasImportadas { get; set; } = new();
    }
}
