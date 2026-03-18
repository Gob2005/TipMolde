using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TipMolde.Core.Models
{
    public class Peca
    {
        public int Peca_id { get; set; }
        public required Molde Molde { get; set; }
        public int Numero_peca { get; set; }
        public int Prioridade { get; set; }
        public string? Descricao { get; set; }
    }
}
