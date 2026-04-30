using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.RelatorioDto
{
    /// <summary>
    /// Representa o contexto base comum ao preenchimento das fichas exportadas.
    /// </summary>
    /// <remarks>
    /// Este DTO concentra apenas os campos transversais a varios templates de relatorio.
    /// Dados especificos de cada tipo de ficha devem viver em read-models dedicados.
    /// </remarks>
    public sealed class FichaRelatorioBaseDto
    {
        public int FichaId { get; set; }
        public TipoFicha Tipo { get; set; }
        public string MoldeNumero { get; set; } = string.Empty;
        public string? MoldeNome { get; set; }
        public string? NumeroMoldeCliente { get; set; }
        public string? ImagemCapaPath { get; set; }
        public int NumeroCavidades { get; set; }
        public TipoPedido TipoPedido { get; set; }
        public string ClienteNome { get; set; } = string.Empty;
        public string? NomeServicoCliente { get; set; }
        public string? NumeroProjetoCliente { get; set; }
        public string? NomeResponsavelCliente { get; set; }
        public DateTime DataEntregaPrevista { get; set; }
        public string? MaterialInjecao { get; set; }
        public decimal? Contracao { get; set; }
        public string? TipoInjecao { get; set; }
        public string? AcabamentoPeca { get; set; }
        public string? MaterialMacho { get; set; }
        public string? MaterialCavidade { get; set; }
        public string? MaterialMovimentos { get; set; }
        public string? SistemaInjecao { get; set; }
        public CorMolde? Cor { get; set; }
        public bool LadoFixo { get; set; }
        public bool LadoMovel { get; set; }
    }
}
