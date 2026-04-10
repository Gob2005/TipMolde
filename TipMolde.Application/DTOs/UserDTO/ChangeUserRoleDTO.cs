using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.DTOs.UserDTO
{
    public class ChangeUserRoleDTO
    {
        [Required]
        public required UserRole Role { get; set; }
    }
}
