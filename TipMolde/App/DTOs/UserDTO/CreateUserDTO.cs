using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TipMolde.App.DTOs.UserDTO
{
    public class CreateUserDTO
    {
        [Required, MinLength(5), MaxLength(100)]
        public required string Nome { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public required string Email { get; set; }

        [Required, MinLength(8), MaxLength(255)]
        public required string Password { get; set; }

        public enum UserRole
        {
            Admin,
            Responsavel_Encomendas,
            Engenheiro,
            Producao
        }

        public required UserRole Role { get; set; }
    }
}
