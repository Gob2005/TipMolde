namespace TipMolde.Core.Models.Producao
{
    public class Peca
{
    public int Peca_id { get; set; }
    public required string Designacao { get; set; }
    public int Prioridade { get; set; }
    public string? MaterialDesignacao { get; set; }
    public bool MaterialRecebido { get; set; }

    public int Molde_id { get; set; }
    public Molde? Molde { get; set; }
}

}
