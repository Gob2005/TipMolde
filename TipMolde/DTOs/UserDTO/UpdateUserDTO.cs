using System.ComponentModel.DataAnnotations;

namespace TipMolde.API.DTOs.UserDTO
{
    public class UpdateUserDTO
    {
        [Required]
        public int Id { get; set; }

        [MinLength(5), MaxLength(100)]
        public string? Nome { get; set; }

        [EmailAddress, MaxLength(150)]
        public string? Email { get; set; }

        [MinLength(8), MaxLength(255)]
        public string? Password { get; set; }

        public enum UserRole
        {
            Admin,
            Responsavel_Encomendas,
            Engenheiro,
            Producao
        }
        public UserRole? Role { get; set; }
    }
}
