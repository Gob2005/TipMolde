using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TipMolde.API.DTOs.UserDTO
{
    public class ResponseUserDTO
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public enum UserRole
        {
            Admin,
            Responsavel_Encomendas,
            Engenheiro,
            Producao
        }
        public required UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
