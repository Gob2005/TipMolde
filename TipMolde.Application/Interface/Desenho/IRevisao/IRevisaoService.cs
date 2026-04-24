using TipMolde.Application.DTOs.RevisaoDTO;

namespace TipMolde.Application.Interface.Desenho.IRevisao
{
    /// <summary>
    /// Define os casos de uso da feature Revisao.
    /// </summary>
    public interface IRevisaoService
    {
        /// <summary>
        /// Lista revisoes associadas a um projeto.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <param name="page">Numero da pagina a ser retornada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao de DTOs de revisao ordenados por numero decrescente.</returns>
        Task<PagedResult<ResponseRevisaoDTO>> GetByProjetoIdAsync(int projetoId, int page = 1, int pageSize = 10);

        /// <summary>
        /// Obtem uma revisao por identificador.
        /// </summary>
        /// <param name="id">Identificador interno da revisao.</param>
        /// <returns>DTO da revisao quando encontrada; nulo caso contrario.</returns>
        Task<ResponseRevisaoDTO?> GetByIdAsync(int id);

        /// <summary>
        /// Cria uma nova revisao para um projeto.
        /// </summary>
        /// <param name="dto">Dados de criacao da revisao.</param>
        /// <returns>DTO da revisao criada.</returns>
        Task<ResponseRevisaoDTO> CreateAsync(CreateRevisaoDTO dto);

        /// <summary>
        /// Regista a primeira resposta do cliente a uma revisao enviada.
        /// </summary>
        /// <param name="revisaoId">Identificador da revisao.</param>
        /// <param name="dto">Payload de resposta do cliente.</param>
        /// <returns>Task de conclusao da operacao.</returns>
        Task UpdateRespostaClienteAsync(int revisaoId, UpdateRespostaRevisaoDTO dto);

        /// <summary>
        /// Remove uma revisao existente.
        /// </summary>
        /// <param name="id">Identificador da revisao a remover.</param>
        /// <returns>Task de conclusao da remocao.</returns>
        Task DeleteAsync(int id);
    }
}
