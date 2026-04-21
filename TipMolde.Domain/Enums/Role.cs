namespace TipMolde.Domain.Enums
{
    /// <summary>
    /// Define os perfis de acesso disponiveis no sistema.
    /// </summary>
    /// <remarks>
    /// Os valores deste enum suportam o controlo de acessos baseado em perfis (RBAC).
    /// </remarks>
    public enum UserRole
    {
        /// <summary>
        /// Perfil com acesso administrativo global.
        /// </summary>
        ADMIN,

        /// <summary>
        /// Perfil responsavel por operacoes comerciais.
        /// </summary>
        GESTOR_COMERCIAL,

        /// <summary>
        /// Perfil responsavel por operacoes de desenho.
        /// </summary>
        GESTOR_DESENHO,

        /// <summary>
        /// Perfil responsavel por operacoes de producao.
        /// </summary>
        GESTOR_PRODUCAO
    }
}
