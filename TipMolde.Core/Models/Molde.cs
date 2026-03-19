using TipMolde.Core.Enums;

namespace TipMolde.Core.Models
{
    public class Molde
    {
        public int Molde_id { get; set; }
        public required string Material { get; set; }
        public required string Dimensoes_molde { get; set; }
        public decimal Peso_estimado { get; set; }
        public int Numero_cavidades { get; set; } = 1;
        public string? NumeroMoldeCliente { get; set; }
        public string? NomeServico { get; set; }
        public TipoMolde TipoMolde { get; set; } = TipoMolde.MONOCOLOR;
        public string? TipoInjecao { get; set; }
        public string? AcabamentoPeca { get; set; }
        public string? MaterialMacho { get; set; }
        public string? MaterialCavidade { get; set; }
        public string? MaterialMovimentos { get; set; }
        public string? SistemaInjecao { get; set; }
        public bool PlacaIsolamentoLadoFixo { get; set; } = false;
        public bool PlacaIsolamentoLadoMovel { get; set; } = false;
        public decimal? Contraccao { get; set; }
        public string? OutrasIndicacoes { get; set; }
        public string? ResponsavelTecnico { get; set; }
        public DateTime? DataEntregaPrevista { get; set; }
        public TipoPedido TipoPedido { get; set; } = TipoPedido.NOVO;


        public int Encomenda_id { get; set; }
        public Encomenda? Encomenda { get; set; }

        // Auto-referência opcional — só preenchida se for TipoPedido.REPARACAO
        public int? MoldeOriginal_id { get; set; }
        public Molde? MoldeOriginal { get; set; }

        public ICollection<Peca> Pecas { get; set; } = new List<Peca>();
    }
}