using AutoMapper;
using TipMolde.Application.DTOs.ClienteDTO;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Service
{
    /// <summary>
    /// Implementa os casos de uso de negocio para gestao de clientes.
    /// </summary>
    /// <remarks>
    /// Aplica validacoes funcionais, regras de unicidade e delega operacoes de persistencia ao repositorio.
    /// </remarks>
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Construtor de ClienteService.
        /// </summary>
        /// <param name="clienteRepository">Repositorio responsavel pelo acesso aos dados de cliente.</param>
        /// <param name="mapper">Mapeador de objetos para conversao entre DTOs e entidades.</param>
        public ClienteService(IClienteRepository clienteRepository, IMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Lista clientes com paginacao.
        /// </summary>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com clientes e metadados de navegacao.</returns>
        public async Task<PagedResult<ResponseClienteDTO>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var result = await _clienteRepository.GetAllAsync(page, pageSize);
            var mappedItems = _mapper.Map<IEnumerable<ResponseClienteDTO>>(result.Items);

            return new PagedResult<ResponseClienteDTO>(
                mappedItems,
                result.TotalCount,
                result.CurrentPage,
                result.PageSize);
        }

        /// <summary>
        /// Obtem um cliente pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do cliente.</param>
        /// <returns>Cliente encontrado ou nulo quando nao existe registo.</returns>
        public async Task<ResponseClienteDTO?> GetByIdAsync(int id)
        {
            var entity = await _clienteRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ResponseClienteDTO>(entity);
        }

        /// <summary>
        /// Pesquisa clientes por nome.
        /// </summary>
        /// <remarks>
        /// Quando o termo de pesquisa e vazio devolve colecao vazia para evitar consulta desnecessaria.
        /// </remarks>
        /// <param name="searchTerm">Termo parcial para pesquisa no nome.</param>
        /// <returns>Colecao de clientes que correspondem ao termo informado.</returns>
        public async Task<PagedResult<ResponseClienteDTO>> SearchByNameAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return CreateEmptyPage(page, pageSize);

            var result = await _clienteRepository.SearchByNameAsync(searchTerm.Trim(), page, pageSize);
            var mappedItems = _mapper.Map<IEnumerable<ResponseClienteDTO>>(result.Items);

            return new PagedResult<ResponseClienteDTO>(
                mappedItems,
                result.TotalCount,
                result.CurrentPage,
                result.PageSize);
        }

        /// <summary>
        /// Pesquisa clientes por sigla.
        /// </summary>
        /// <remarks>
        /// Quando o termo de pesquisa e vazio devolve colecao vazia para evitar consulta desnecessaria.
        /// </remarks>
        /// <param name="searchTerm">Termo parcial para pesquisa na sigla.</param>
        /// <returns>Colecao de clientes que correspondem ao termo informado.</returns>
        public async Task<PagedResult<ResponseClienteDTO>> SearchBySiglaAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return CreateEmptyPage(page, pageSize);

            var result = await _clienteRepository.SearchBySiglaAsync(searchTerm.Trim(), page, pageSize);
            var mappedItems = _mapper.Map<IEnumerable<ResponseClienteDTO>>(result.Items);

            return new PagedResult<ResponseClienteDTO>(
                mappedItems,
                result.TotalCount,
                result.CurrentPage,
                result.PageSize);
        }

        /// <summary>
        /// Obtem um cliente com as encomendas associadas.
        /// </summary>
        /// <param name="clienteId">Identificador unico do cliente.</param>
        /// <returns>Cliente com encomendas ou nulo quando nao existe registo.</returns>
        public async Task<ResponseClienteWithEncomendasDTO?> GetClienteWithEncomendasAsync(int clienteId)
        {
            var entity = await _clienteRepository.GetClienteWithEncomendasAsync(clienteId);
            return entity == null ? null : _mapper.Map<ResponseClienteWithEncomendasDTO>(entity);
        }

        /// <summary>
        /// Cria um novo cliente apos validacoes de obrigatoriedade e unicidade.
        /// </summary>
        /// <remarks>
        /// Fluxo principal:
        /// 1. Valida campos obrigatorios Nome, NIF e Sigla.
        /// 2. Garante unicidade de NIF e Sigla.
        /// 3. Normaliza campos textuais removendo espacos nas extremidades.
        /// 4. Persiste o cliente.
        /// </remarks>
        /// <param name="dto">DTO com dados do cliente a validar e persistir.</param>
        /// <returns>Cliente criado apos validacao e persistencia.</returns>
        public async Task<ResponseClienteDTO> CreateAsync(CreateClienteDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new ArgumentException("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.NIF))
                throw new ArgumentException("NIF e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.Sigla))
                throw new ArgumentException("Sigla e obrigatoria.");

            var nifExists = await _clienteRepository.GetByNifAsync(dto.NIF.Trim());
            if (nifExists != null)
                throw new ArgumentException("Ja existe cliente com este NIF.");

            var siglaExists = await _clienteRepository.GetBySiglaAsync(dto.Sigla.Trim());
            if (siglaExists != null)
                throw new ArgumentException("Ja existe cliente com esta Sigla.");

            var cliente = new Cliente
            {
                Nome = dto.Nome.Trim(),
                NIF = dto.NIF.Trim(),
                Sigla = dto.Sigla.Trim()
            };

            var entity = _mapper.Map<Cliente>(dto);
            await _clienteRepository.AddAsync(entity);
            return _mapper.Map<ResponseClienteDTO>(entity);
        }

        /// <summary>
        /// Atualiza dados de um cliente existente com validacao de unicidade.
        /// </summary>
        /// <remarks>
        /// Fluxo principal:
        /// 1. Confirma existencia do cliente.
        /// 2. Valida conflito de NIF e Sigla quando houver alteracao desses campos.
        /// 3. Atualiza apenas campos informados, preservando os valores existentes quando em branco.
        /// 4. Persiste as alteracoes.
        /// </remarks>
        /// <param name="id">Identificador unico do cliente a atualizar.</param>
        /// <param name="dto">DTO com os dados a atualizar no cliente existente.</param>
        /// <returns>Task assincrona concluida apos atualizacao do cliente.</returns>
        public async Task UpdateAsync(int id, UpdateClienteDTO dto)
        {
            var existing = await _clienteRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Cliente com ID {id} não encontrado.");
            if (!string.IsNullOrWhiteSpace(dto.NIF) && dto.NIF != existing.NIF)
            {
                var nifExists = await _clienteRepository.GetByNifAsync(dto.NIF.Trim());
                if (nifExists != null && nifExists.Cliente_id != existing.Cliente_id)
                    throw new ArgumentException("Ja existe cliente com este NIF.");
            }

            if (!string.IsNullOrWhiteSpace(dto.Sigla) && dto.Sigla != existing.Sigla)
            {
                var siglaExists = await _clienteRepository.GetBySiglaAsync(dto.Sigla.Trim());
                if (siglaExists != null && siglaExists.Cliente_id != existing.Cliente_id)
                    throw new ArgumentException("Ja existe cliente com esta Sigla.");
            }

            existing.Nome = string.IsNullOrWhiteSpace(dto.Nome) ? existing.Nome : dto.Nome.Trim();
            existing.Pais = string.IsNullOrWhiteSpace(dto.Pais) ? existing.Pais : dto.Pais.Trim();
            existing.Email = string.IsNullOrWhiteSpace(dto.Email) ? existing.Email : dto.Email.Trim();
            existing.Telefone = string.IsNullOrWhiteSpace(dto.Telefone) ? existing.Telefone : dto.Telefone.Trim();
            existing.NIF = string.IsNullOrWhiteSpace(dto.NIF) ? existing.NIF : dto.NIF.Trim();
            existing.Sigla = string.IsNullOrWhiteSpace(dto.Sigla) ? existing.Sigla : dto.Sigla.Trim();

            await _clienteRepository.UpdateAsync(existing);
        }

        /// <summary>
        /// Remove um cliente pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do cliente a remover.</param>
        /// <returns>Task assincrona concluida apos remocao do cliente.</returns>
        public async Task DeleteAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null) throw new KeyNotFoundException($"Cliente com ID {id} nao encontrado.");
            await _clienteRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Cria um resultado paginado vazio com limites de pagina normalizados.
        /// </summary>
        /// <param name="page">Numero de pagina solicitado pelo consumidor.</param>
        /// <param name="pageSize">Quantidade de itens por pagina solicitada pelo consumidor.</param>
        /// <returns>Resultado paginado sem itens e com metadados consistentes.</returns>
        private static PagedResult<ResponseClienteDTO> CreateEmptyPage(int page, int pageSize)
        {
            var normalizedPage = page < 1 ? 1 : page;
            var normalizedPageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;

            return new PagedResult<ResponseClienteDTO>(
                Enumerable.Empty<ResponseClienteDTO>(),
                0,
                normalizedPage,
                normalizedPageSize);
        }
    }
}
