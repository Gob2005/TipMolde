using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Application.Interface.Desenho.IRegistoTempoProjeto
{
    /// <summary>
    /// Define operacoes de persistencia especificas para RegistoTempoProjeto.
    /// </summary>
    public interface IRegistoTempoProjetoRepository : IGenericRepository<RegistoTempoProjeto, int>
    {
        /// <summary>
        /// Lista o historico temporal de um projeto para um autor.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <param name="autorId">Identificador do autor.</param>
        /// <param name="page">Numero da pagina a ser retornada.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Colecao ordenada de registos do historico.</returns>
        Task<PagedResult<RegistoTempoProjeto>> GetHistoricoAsync(int projetoId, int autorId, int page, int pageSize);

        /// <summary>
        /// Obtem o ultimo registo temporal persistido para um projeto e autor.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <param name="autorId">Identificador do autor.</param>
        /// <returns>Ultimo registo encontrado; nulo quando ainda nao existe historico.</returns>
        Task<RegistoTempoProjeto?> GetUltimoRegistoAsync(int projetoId, int autorId);
    }
}
