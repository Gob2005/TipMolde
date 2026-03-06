namespace TipMolde.API.DTOs.MoldeDTO
{
    public class UpdateMoldeDTO
    {
        public string? Material { get; set; }
        public string? Dimensoes_molde { get; set; }
        public decimal? Peso_estimado { get; set; }
        public int? Numero_cavidades { get; set; }
    }
}
