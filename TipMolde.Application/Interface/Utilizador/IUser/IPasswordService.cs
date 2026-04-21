namespace TipMolde.Application.Interface.Utilizador.IUser
{
    /// <summary>
    /// Define operacoes de gestao de passwords de utilizador.
    /// </summary>
    /// <remarks>
    /// Separa alteracoes de credenciais das operacoes CRUD para respeitar o principio de responsabilidade unica.
    /// </remarks>
    public interface IPasswordService
    {
        /// <summary>
        /// Altera a password de um utilizador autenticado.
        /// </summary>
        /// <param name="userId">Identificador do utilizador que solicita a alteracao.</param>
        /// <param name="currentPassword">Password atual para validacao de identidade.</param>
        /// <param name="newPassword">Nova password a aplicar apos validacao.</param>
        /// <returns>Task assincrona concluida apos atualizacao da password.</returns>
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);

        /// <summary>
        /// Repõe a password de um utilizador sem exigir password atual.
        /// </summary>
        /// <param name="userId">Identificador do utilizador alvo da reposicao.</param>
        /// <param name="newPassword">Nova password a definir.</param>
        /// <returns>Task assincrona concluida apos atualizacao da password.</returns>
        Task ResetPasswordAsync(int userId, string newPassword);
    }
}
