namespace TipMolde.App.DTOs.MoldeDTO
{
    public class ResponseMoldeDTO
    {
        public int MoldeId { get; set; }
        public int ClienteId { get; set; }
        public required string Material { get; set; }
        public string? Dimensoes_molde { get; set; }
        public decimal Peso_estimado { get; set; }
        public int Numero_cavidades { get; set; }
    }
}
