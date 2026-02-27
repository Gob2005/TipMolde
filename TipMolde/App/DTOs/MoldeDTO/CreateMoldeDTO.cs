namespace TipMolde.App.DTOs.MoldeDTO
{
    public class CreateMoldeDTO
    {
        public int ClienteId { get; set; }
        public string Material { get; set; }
        public string? Dimensoes_molde { get; set; }
        public decimal Peso_estimado { get; set; }
        public int Numero_cavidades { get; set; } = 1;
    }
}
