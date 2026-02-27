namespace TipMolde.App.DTOs.UserDTO
{
    public class UpdateUserDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public enum UserRole
        {
            Admin,
            Responsavel_Encomendas,
            Engenheiro,
            Producao
        }

        public UserRole Role { get; set; }
    }
}
