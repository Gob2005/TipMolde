using TipMolde.Domain.Entities;

namespace TipMolde.Application.Interface.Utilizador.IUser
{
    /// <summary>
    /// Define operacoes de persistencia para utilizadores.
    /// </summary>
    /// <remarks>
    /// Expande o repositorio generico com consultas especificas por nome e email.
    /// </remarks>
    public interface IUserRepository : IGenericRepository<User, int>
    {
        /// <summary>
        /// Pesquisa utilizadores por nome.
        /// </summary>
        /// <param name="searchTerm">Termo parcial para pesquisa no nome do utilizador.</param>
        /// <param name="page">Numero da pagina a ser retornada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao de utilizadores que correspondem ao termo informado.</returns>
        Task<PagedResult<User>> SearchByNameAsync(string searchTerm, int page, int pageSize);

        /// <summary>
        /// Obtem um utilizador pelo email.
        /// </summary>
        /// <param name="email">Email unico do utilizador.</param>
        /// <returns>Utilizador encontrado ou nulo quando nao existe correspondencia.</returns>
        Task<User?> GetByEmailAsync(string email);
    }
}
