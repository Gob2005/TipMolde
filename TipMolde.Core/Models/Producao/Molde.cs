using TipMolde.Core.Enums;
using TipMolde.Core.Models.Comercio;

namespace TipMolde.Core.Models.Producao
{
    public class Molde
    {
        public int Molde_id { get; set; }
        public required string Numero { get; set; }
        public string? NumeroMoldeCliente { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public int Numero_cavidades { get; set; }
        public TipoPedido TipoPedido { get; set; }

        public EspecificacoesTecnicas? Especificacoes { get; set; }
        public ICollection<Peca> Pecas { get; set; } = new List<Peca>();
        public ICollection<EncomendaMolde> EncomendasMoldes { get; set; } = new List<EncomendaMolde>();
    }

}