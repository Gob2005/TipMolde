using TipMolde.Application.Dtos.FasesProducaoDto;
using TipMolde.Application.Interface;

namespace TipMolde.Application.Interface.Producao.IFasesProducao
{
    /// <summary>
    /// Define os casos de uso publicos da feature FasesProducao.
    /// </summary>
    /// <remarks>
    /// O contrato expoe apenas DTOs para evitar acoplamento direto entre API e entidades de dominio.
    /// </remarks>
    public interface IFasesProducaoService
    {
        /// <summary>
        /// Lista fases de producao com paginacao.
        /// </summary>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com DTOs de resposta.</returns>
        Task<PagedResult<ResponseFasesProducaoDto>> GetAllAsync(int page = 1, int pageSize = 10);

        /// <summary>
        /// Obtem uma fase de producao por identificador.
        /// </summary>
        /// <param name="id">Identificador interno da fase.</param>
        /// <returns>DTO da fase quando encontrada; nulo caso contrario.</returns>
        Task<ResponseFasesProducaoDto?> GetByIdAsync(int id);

        /// <summary>
        /// Cria uma nova fase de producao.
        /// </summary>
        /// <remarks>
        /// Fluxo critico:
        /// 1. Valida nome explicito.
        /// 2. Valida unicidade funcional.
        /// 3. Persiste a fase.
        /// </remarks>
        /// <param name="dto">Dados de criacao da fase.</param>
        /// <returns>DTO da fase criada.</returns>
        Task<ResponseFasesProducaoDto> CreateAsync(CreateFasesProducaoDto dto);

        /// <summary>
        /// Atualiza parcialmente uma fase existente.
        /// </summary>
        /// <remarks>
        /// Campos nao enviados devem manter o valor atual.
        /// </remarks>
        /// <param name="id">Identificador da fase a atualizar.</param>
        /// <param name="dto">Dados de atualizacao parcial.</param>
        /// <returns>Task de conclusao da atualizacao.</returns>
        Task UpdateAsync(int id, UpdateFasesProducaoDto dto);

        /// <summary>
        /// Remove uma fase existente.
        /// </summary>
        /// <remarks>
        /// A remocao deve falhar quando existirem maquinas associadas.
        /// </remarks>
        /// <param name="id">Identificador da fase a remover.</param>
        /// <returns>Task de conclusao da remocao.</returns>
        Task DeleteAsync(int id);
    }
}
