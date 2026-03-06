namespace TipMolde.Core.Models
{
    public class Molde
    {
        public int Molde_id { get; set; }
        public required string Material { get; set; }
        public required string Dimensoes_molde { get; set; }
        public decimal Peso_estimado { get; set; }
        public int Numero_cavidades { get; set; } = 1;
        public Cliente? Cliente { get; set; }
    }
}