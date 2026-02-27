namespace TipMolde.App.DTOs.UserDTO
{
    public class ChangeUserRoleDTO
    {
        public int Id { get; set; }
        public enum UserRole
        {
            Admin,
            Responsavel_Encomendas,
            Engenheiro,
            Producao
        }

        public UserRole ?Role { get; set; }
    }
}
