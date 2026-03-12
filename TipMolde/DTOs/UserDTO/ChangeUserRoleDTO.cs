using System.ComponentModel.DataAnnotations;
using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.UserDTO
{
    public class ChangeUserRoleDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public required UserRole Role { get; set; }
    }
}
