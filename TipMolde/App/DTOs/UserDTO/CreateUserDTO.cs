using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TipMolde.App.DTOs.UserDTO
{
    public class CreateUserDTO
    {
        public string ?Nome { get; set; }
        public string ?Email { get; set; }
        public string ?Password { get; set; }

        public enum UserRole
        {
            Admin,
            Responsavel_Encomendas,
            Engenheiro,
            Producao
        }

        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
