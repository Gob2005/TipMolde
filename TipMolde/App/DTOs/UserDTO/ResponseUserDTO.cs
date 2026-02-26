using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TipMolde.App.DTOs.UserDTO
{
    public class ResponseUserDTO
    {
        public int Id { get; set; }
        public string ?Nome { get; set; }
        public string ?Email { get; set; }
        public string ?Password { get; set; }
        public enum Role
        {
            Admin,
            Responsavel_Encomendas,
            Engenheiro,
            Producao
        }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
