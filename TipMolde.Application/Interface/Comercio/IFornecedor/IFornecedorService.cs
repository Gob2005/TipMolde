using TipMolde.Application.DTOs.FornecedorDTO;

namespace TipMolde.Application.Interface.Comercio.IFornecedor
{
    /// <summary>
    /// Define os casos de uso de negocio para gestao de fornecedores.
    /// </summary>
    /// <remarks>
    /// Centraliza operacoes de consulta, pesquisa, criacao, atualizacao e remocao de fornecedores.
    /// </remarks>
    public interface IFornecedorService
    {
        /// <summary>
        /// Lista fornecedores com paginacao.
        /// </summary>
        /// <param name="page">Numero da pagina solicitada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com fornecedores e metadados de navegacao.</returns>
        Task<PagedResult<ResponseFornecedorDTO>> GetAllAsync(int page = 1, int pageSize = 10);

        /// <summary>
        /// Obtem um fornecedor pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do fornecedor.</param>
        /// <returns>Fornecedor encontrado ou nulo quando nao existe registo.</returns>
        Task<ResponseFornecedorDTO?> GetByIdAsync(int id);

        /// <summary>
        /// Pesquisa fornecedores por nome.
        /// </summary>
        /// <param name="searchTerm">Termo parcial de pesquisa por nome.</param>
        /// <param name="page">Numero da pagina solicitada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao paginada de fornecedores que correspondem ao termo informado.</returns>
        Task<PagedResult<ResponseFornecedorDTO>> SearchByNameAsync(string searchTerm, int page = 1, int pageSize = 10);

        /// <summary>
        /// Cria um novo fornecedor.
        /// </summary>
        /// <param name="dto">DTO com dados do fornecedor a persistir.</param>
        /// <returns>DTO de resposta do fornecedor apos validacao e persistencia.</returns>
        Task<ResponseFornecedorDTO> CreateAsync(CreateFornecedorDTO dto);

        /// <summary>
        /// Atualiza os dados de um fornecedor existente.
        /// </summary>
        /// <param name="id">Identificador unico do fornecedor a atualizar.</param>
        /// <param name="dto">DTO com os dados a atualizar no fornecedor.</param>
        /// <returns>Task assincrona concluida apos atualizacao do fornecedor.</returns>
        Task UpdateAsync(int id, UpdateFornecedorDTO dto);

        /// <summary>
        /// Remove um fornecedor pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do fornecedor a remover.</param>
        /// <returns>Task assincrona concluida apos remocao do fornecedor.</returns>
        Task DeleteAsync(int id);
    }
}