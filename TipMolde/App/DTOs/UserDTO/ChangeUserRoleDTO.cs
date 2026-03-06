using System.ComponentModel.DataAnnotations;

namespace TipMolde.App.DTOs.UserDTO
{
    public class ChangeUserRoleDTO
    {
        [Required]
        public int Id { get; set; }
        public enum UserRole
        {
            Admin,
            Responsavel_Encomendas,
            Engenheiro,
            Producao
        }

        [Required]
        public required UserRole Role { get; set; }
    }
}
