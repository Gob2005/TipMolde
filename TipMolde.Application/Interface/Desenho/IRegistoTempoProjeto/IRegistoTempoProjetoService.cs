using TipMolde.Application.Dtos.RegistoTempoProjetoDto;

namespace TipMolde.Application.Interface.Desenho.IRegistoTempoProjeto
{
    /// <summary>
    /// Define os casos de uso da feature RegistoTempoProjeto.
    /// </summary>
    public interface IRegistoTempoProjetoService
    {
        /// <summary>
        /// Lista o historico de tempo de um projeto para um autor.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <param name="autorId">Identificador do autor.</param>
        /// <param name="page">Numero da pagina a ser retornada.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Colecao ordenada de registos de tempo em DTO.</returns>
        Task<PagedResult<ResponseRegistoTempoProjetoDto>> GetHistoricoAsync(int projetoId, int autorId, int page = 1, int pageSize = 10);

        /// <summary>
        /// Obtem um registo de tempo por identificador.
        /// </summary>
        /// <param name="id">Identificador interno do registo.</param>
        /// <returns>DTO do registo quando encontrado; nulo caso contrario.</returns>
        Task<ResponseRegistoTempoProjetoDto?> GetByIdAsync(int id);

        /// <summary>
        /// Cria um novo evento no historico temporal do projeto.
        /// </summary>
        /// <param name="dto">Dados de criacao do registo.</param>
        /// <returns>DTO do registo criado.</returns>
        Task<ResponseRegistoTempoProjetoDto> CreateRegistoAsync(CreateRegistoTempoProjetoDto dto);

        /// <summary>
        /// Remove um registo de tempo existente.
        /// </summary>
        /// <param name="id">Identificador interno do registo.</param>
        /// <returns>Task de conclusao da remocao.</returns>
        Task DeleteAsync(int id);
    }
}
