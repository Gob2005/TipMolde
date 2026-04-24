using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.MoldeDto
{
    /// <summary>
    /// Representa a resposta publica da feature Molde.
    /// </summary>
    public class ResponseMoldeDto
    {
        public int MoldeId { get; set; }
        public string? Numero { get; set; }
        public string? NumeroMoldeCliente { get; set; }
        public string? Nome { get; set; }
        public string? ImagemCapaPath { get; set; }
        public string? Descricao { get; set; }
        public int Numero_cavidades { get; set; }
        public TipoPedido TipoPedido { get; set; }
        public decimal? Largura { get; set; }
        public decimal? Comprimento { get; set; }
        public decimal? Altura { get; set; }
        public decimal? PesoEstimado { get; set; }
        public string? TipoInjecao { get; set; }
        public string? SistemaInjecao { get; set; }
        public decimal? Contracao { get; set; }
        public string? AcabamentoPeca { get; set; }
        public CorMolde? Cor { get; set; }
        public string? MaterialMacho { get; set; }
        public string? MaterialCavidade { get; set; }
        public string? MaterialMovimentos { get; set; }
        public string? MaterialInjecao { get; set; }
    }
}
