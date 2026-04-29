using TipMolde.Application.Dtos.ProjetoDto;

namespace TipMolde.Application.Interface.Desenho.IProjeto
{
    /// <summary>
    /// Define os casos de uso publicos da feature Projeto.
    /// </summary>
    /// <remarks>
    /// O contrato expoe Dtos para evitar acoplamento direto entre API e entidades de dominio.
    /// </remarks>
    public interface IProjetoService
    {
        /// <summary>
        /// Lista projetos de forma paginada.
        /// </summary>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com projetos.</returns>
        Task<PagedResult<ResponseProjetoDto>> GetAllAsync(int page = 1, int pageSize = 10);

        /// <summary>
        /// Obtem um projeto por identificador.
        /// </summary>
        /// <param name="id">Identificador interno do projeto.</param>
        /// <returns>DTO do projeto quando encontrado; nulo caso contrario.</returns>
        Task<ResponseProjetoDto?> GetByIdAsync(int id);

        /// <summary>
        /// Obtem um projeto com as revisoes associadas.
        /// </summary>
        /// <param name="id">Identificador interno do projeto.</param>
        /// <returns>DTO enriquecido com revisoes quando encontrado; nulo caso contrario.</returns>
        Task<ResponseProjetoWithRevisoesDto?> GetWithRevisoesAsync(int id);

        /// <summary>
        /// Lista projetos associados a um molde.
        /// </summary>
        /// <param name="moldeId">Identificador do molde.</param>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com projetos associados.</returns>
        Task<PagedResult<ResponseProjetoDto>> GetByMoldeIdAsync(int moldeId, int page = 1, int pageSize = 10);

        /// <summary>
        /// Cria um novo projeto.
        /// </summary>
        /// <param name="dto">Dados de criacao do projeto.</param>
        /// <returns>DTO do projeto criado.</returns>
        Task<ResponseProjetoDto> CreateAsync(CreateProjetoDto dto);

        /// <summary>
        /// Atualiza parcialmente um projeto existente.
        /// </summary>
        /// <remarks>
        /// Campos nao enviados no DTO devem manter o valor atual do agregado.
        /// </remarks>
        /// <param name="id">Identificador do projeto a atualizar.</param>
        /// <param name="dto">Dados de atualizacao parcial.</param>
        /// <returns>Task de conclusao da atualizacao.</returns>
        Task UpdateAsync(int id, UpdateProjetoDto dto);

        /// <summary>
        /// Remove um projeto existente.
        /// </summary>
        /// <param name="id">Identificador do projeto a remover.</param>
        /// <returns>Task de conclusao da remocao.</returns>
        Task DeleteAsync(int id);
    }
}
