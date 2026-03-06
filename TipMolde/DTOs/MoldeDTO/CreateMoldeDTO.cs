namespace TipMolde.API.DTOs.MoldeDTO
{
    public class CreateMoldeDTO
    {
        public int ClienteId { get; set; }
        public required string Material { get; set; }
        public required string Dimensoes_molde { get; set; }
        public decimal Peso_estimado { get; set; }
        public int Numero_cavidades { get; set; } = 1;
    }
}
