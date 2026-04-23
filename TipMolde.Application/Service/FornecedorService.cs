using AutoMapper;
using TipMolde.Application.DTOs.FornecedorDTO;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.IFornecedor;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Service
{
    /// <summary>
    /// Implementa os casos de uso de negocio para gestao de fornecedores.
    /// </summary>
    /// <remarks>
    /// Aplica validacoes funcionais, regras de unicidade e delega operacoes de persistencia ao repositorio.
    /// </remarks>
    public class FornecedorService : IFornecedorService
    {
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Construtor de FornecedorService.
        /// </summary>
        /// <param name="fornecedorRepository">Repositorio responsavel pelo acesso aos dados de fornecedor.</param>
        /// <param name="mapper">Mapeador de objetos para conversao entre DTOs e entidades.</param>
        public FornecedorService(IFornecedorRepository fornecedorRepository, IMapper mapper)
        {
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Lista fornecedores com paginacao.
        /// </summary>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com fornecedores e metadados de navegacao.</returns>
        public async Task<PagedResult<ResponseFornecedorDTO>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var result = await _fornecedorRepository.GetAllAsync(page, pageSize);
            var mappedItems = _mapper.Map<IEnumerable<ResponseFornecedorDTO>>(result.Items);

            return new PagedResult<ResponseFornecedorDTO>(
                mappedItems,
                result.TotalCount,
                result.CurrentPage,
                result.PageSize);
        }

        /// <summary>
        /// Obtem um fornecedor pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do fornecedor.</param>
        /// <returns>Fornecedor encontrado ou nulo quando nao existe registo.</returns>
        public async Task<ResponseFornecedorDTO?> GetByIdAsync(int id)
        {
            var entity = await _fornecedorRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ResponseFornecedorDTO>(entity);
        }

        /// <summary>
        /// Pesquisa fornecedores por nome.
        /// </summary>
        /// <remarks>
        /// Quando o termo de pesquisa e vazio devolve colecao vazia para evitar consulta desnecessaria.
        /// </remarks>
        /// <param name="searchTerm">Termo parcial para pesquisa no nome.</param>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao paginada de fornecedores que correspondem ao termo informado.</returns>
        public async Task<PagedResult<ResponseFornecedorDTO>> SearchByNameAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return CreateEmptyPage(page, pageSize);

            var result = await _fornecedorRepository.SearchByNameAsync(searchTerm.Trim(), page, pageSize);
            var mappedItems = _mapper.Map<IEnumerable<ResponseFornecedorDTO>>(result.Items);

            return new PagedResult<ResponseFornecedorDTO>(
                mappedItems,
                result.TotalCount,
                result.CurrentPage,
                result.PageSize);
        }

        /// <summary>
        /// Cria um novo fornecedor apos validacoes de obrigatoriedade e unicidade.
        /// </summary>
        /// <remarks>
        /// Fluxo principal:
        /// 1. Valida campos obrigatorios Nome e NIF.
        /// 2. Garante unicidade do NIF.
        /// 3. Normaliza campos textuais removendo espacos nas extremidades.
        /// 4. Persiste o fornecedor.
        /// </remarks>
        /// <param name="dto">DTO com dados do fornecedor a validar e persistir.</param>
        /// <returns>Fornecedor criado apos validacao e persistencia.</returns>
        public async Task<ResponseFornecedorDTO> CreateAsync(CreateFornecedorDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.NIF))
                throw new ArgumentException("NIF e obrigatorio.");

            var existing = await _fornecedorRepository.GetByNifAsync(dto.NIF.Trim());
            if (existing != null)
                throw new ArgumentException("Ja existe fornecedor com este NIF.");

            var entity = _mapper.Map<Fornecedor>(dto);

            await _fornecedorRepository.AddAsync(entity);

            return _mapper.Map<ResponseFornecedorDTO>(entity);
        }

        /// <summary>
        /// Atualiza dados de um fornecedor existente com validacao de unicidade.
        /// </summary>
        /// <remarks>
        /// Fluxo principal:
        /// 1. Confirma existencia do fornecedor.
        /// 2. Valida conflito de NIF apenas quando ha alteracao do valor.
        /// 3. Atualiza apenas campos informados, preservando os valores existentes quando o cliente omite ou envia texto em branco.
        /// 4. Persiste as alteracoes.
        /// </remarks>
        /// <param name="id">Identificador unico do fornecedor a atualizar.</param>
        /// <param name="dto">DTO com os dados a atualizar no fornecedor existente.</param>
        /// <returns>Task assincrona concluida apos atualizacao do fornecedor.</returns>
        public async Task UpdateAsync(int id, UpdateFornecedorDTO dto)
        {
            var existing = await _fornecedorRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Fornecedor com ID {id} nao encontrado.");

            // Porque: a validacao de unicidade do NIF so corre quando o valor muda,
            // evitando falso conflito com o proprio registo em updates legitimos.
            if (!string.IsNullOrWhiteSpace(dto.NIF) &&
                !string.Equals(dto.NIF.Trim(), existing.NIF, StringComparison.Ordinal))
            {
                var nifExists = await _fornecedorRepository.GetByNifAsync(dto.NIF.Trim());
                if (nifExists != null && nifExists.Fornecedor_id != existing.Fornecedor_id)
                    throw new ArgumentException("Ja existe fornecedor com este NIF.");
            }

            // Porque: o profile ignora campos nulos ou em branco, preservando os valores atuais
            // quando o update e parcial.
            _mapper.Map(dto, existing);

            await _fornecedorRepository.UpdateAsync(existing);
        }

        /// <summary>
        /// Remove um fornecedor pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do fornecedor a remover.</param>
        /// <returns>Task assincrona concluida apos remocao do fornecedor.</returns>
        public async Task DeleteAsync(int id)
        {
            var fornecedor = await _fornecedorRepository.GetByIdAsync(id);
            if (fornecedor == null)
                throw new KeyNotFoundException($"Fornecedor com ID {id} nao encontrado.");

            await _fornecedorRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Cria um resultado paginado vazio com limites de pagina normalizados.
        /// </summary>
        /// <param name="page">Numero de pagina solicitado pelo consumidor.</param>
        /// <param name="pageSize">Quantidade de itens por pagina solicitada pelo consumidor.</param>
        /// <returns>Resultado paginado sem itens e com metadados consistentes.</returns>
        private static PagedResult<ResponseFornecedorDTO> CreateEmptyPage(int page, int pageSize)
        {
            var normalizedPage = page < 1 ? 1 : page;
            var normalizedPageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;

            return new PagedResult<ResponseFornecedorDTO>(
                Enumerable.Empty<ResponseFornecedorDTO>(),
                0,
                normalizedPage,
                normalizedPageSize);
        }
    }
}