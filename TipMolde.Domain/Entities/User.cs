using TipMolde.Domain.Enums;

namespace TipMolde.Domain.Entities
{
    /// <summary>
    /// Representa um utilizador autenticado do sistema.
    /// Suporta controlo de acessos baseado em perfis (RBAC) através da propriedade Role.
    /// </summary>
    /// <remarks>
    /// A palavra-passe é armazenada em formato hash (PBKDF2) por motivos de segurança.
    /// O email é usado como identificador único de autenticação.
    /// </remarks>
    public class User
    {
        /// <summary>
        /// Identificador unico do utilizador.
        /// </summary>
        public int User_id { get; set; }

        /// <summary>
        /// Nome completo do utilizador.
        /// </summary>
        public required string Nome { get; set; }

        /// <summary>
        /// Email único do utilizador, usado como credencial de autenticação.
        /// Validado ao nível da base de dados por índice único.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Password em formato hash (PBKDF2 com salt).
        /// Nunca armazenar palavras-passe em texto simples.
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// Perfil do utilizador que determina as suas permissões no sistema (RBAC).
        /// </summary>
        public required UserRole Role { get; set; }

        /// <summary>
        /// Data de criação do registo, utilizada para auditoria.
        /// Imutável após criação (não há setter público explícito).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
