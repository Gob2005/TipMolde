using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TipMolde.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public enum role
        {
            Admin,
            Responsavel_Encomendas,
            Engenheiro,
            Producao
        }
        public DateTime CreatedAt { get; set; }
    }
}
