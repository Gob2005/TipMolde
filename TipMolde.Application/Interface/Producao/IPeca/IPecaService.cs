using TipMolde.Application.DTOs.PecaDTO;
using TipMolde.Application.Interface;

namespace TipMolde.Application.Interface.Producao.IPeca
{
    /// <summary>
    /// Define os casos de uso publicos da feature Peca.
    /// </summary>
    /// <remarks>
    /// O contrato expoe DTOs para evitar acoplamento direto entre API e entidades de dominio.
    /// </remarks>
    public interface IPecaService
    {
        /// <summary>
        /// Lista pecas de forma paginada.
        /// </summary>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com pecas.</returns>
        Task<PagedResult<ResponsePecaDTO>> GetAllAsync(int page = 1, int pageSize = 10);

        /// <summary>
        /// Obtem uma peca por identificador.
        /// </summary>
        /// <param name="id">Identificador interno da peca.</param>
        /// <returns>DTO da peca quando encontrada; nulo caso contrario.</returns>
        Task<ResponsePecaDTO?> GetByIdAsync(int id);

        /// <summary>
        /// Lista pecas associadas a um molde.
        /// </summary>
        /// <param name="moldeId">Identificador do molde.</param>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com pecas do molde.</returns>
        Task<PagedResult<ResponsePecaDTO>> GetByMoldeIdAsync(int moldeId, int page = 1, int pageSize = 10);

        /// <summary>
        /// Obtem uma peca pela designacao dentro de um molde.
        /// </summary>
        /// <param name="designacao">Designacao funcional da peca.</param>
        /// <param name="moldeId">Identificador do molde.</param>
        /// <returns>DTO da peca quando encontrada; nulo caso contrario.</returns>
        Task<ResponsePecaDTO?> GetByDesignacaoAsync(string designacao, int moldeId);

        /// <summary>
        /// Cria uma nova peca.
        /// </summary>
        /// <remarks>
        /// Fluxo critico:
        /// 1. Valida molde existente.
        /// 2. Valida designacao obrigatoria.
        /// 3. Garante unicidade da designacao dentro do molde.
        /// 4. Persiste a peca.
        /// </remarks>
        /// <param name="dto">Dados de criacao da peca.</param>
        /// <returns>DTO da peca criada.</returns>
        Task<ResponsePecaDTO> CreateAsync(CreatePecaDTO dto);

        /// <summary>
        /// Atualiza parcialmente uma peca existente.
        /// </summary>
        /// <remarks>
        /// Campos omitidos no DTO devem manter o valor atual da entidade.
        /// </remarks>
        /// <param name="id">Identificador da peca a atualizar.</param>
        /// <param name="dto">Dados de atualizacao parcial.</param>
        /// <returns>Task de conclusao da atualizacao.</returns>
        Task UpdateAsync(int id, UpdatePecaDTO dto);

        /// <summary>
        /// Remove uma peca existente.
        /// </summary>
        /// <param name="id">Identificador da peca a remover.</param>
        /// <returns>Task de conclusao da remocao.</returns>
        Task DeleteAsync(int id);


        //Task<int> ImportarCsvAsync(int moldeId, Stream csvStream);
    }
}

