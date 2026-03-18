using System.ComponentModel.DataAnnotations;
using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.UserDTO
{
    public class UpdateUserDTO
    {
        [MinLength(5), MaxLength(100)]
        public string? Nome { get; set; }

        [EmailAddress, MaxLength(150)]
        public string? Email { get; set; }

        [MinLength(8), MaxLength(255)]
        public string? Password { get; set; }
        public UserRole? Role { get; set; }
    }
}
