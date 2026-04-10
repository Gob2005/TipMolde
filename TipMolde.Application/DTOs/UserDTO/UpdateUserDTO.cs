using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.UserDTO
{
    public class UpdateUserDTO
    {
        [MinLength(3), MaxLength(100)]
        public string? Nome { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }
    }
}
