using AutoMapper;
using Microsoft.Extensions.Logging;
using TipMolde.Application.Dtos.PedidoMaterialDto;
using TipMolde.Application.Exceptions;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.IFornecedor;
using TipMolde.Application.Interface.Comercio.IPedidoMaterial;
using TipMolde.Application.Interface.Producao.IPeca;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Service
{
    /// <summary>
    /// Implementa os casos de uso de negocio para o agregado PedidoMaterial.
    /// </summary>
    /// <remarks>
    /// Este servico garante consistencia entre o pedido, as linhas associadas e o desbloqueio de producao
    /// quando a rececao do material e registada.
    /// </remarks>
    public class PedidoMaterialService : IPedidoMaterialService
    {
        private readonly IPedidoMaterialRepository _pedidoRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IPecaRepository _pecaRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PedidoMaterialService> _logger;

        /// <summary>
        /// Construtor de PedidoMaterialService.
        /// </summary>
        /// <param name="pedidoRepository">Repositorio responsavel pela persistencia de pedidos de material.</param>
        /// <param name="fornecedorRepository">Repositorio usado para validar a existencia do fornecedor.</param>
        /// <param name="pecaRepository">Repositorio usado para validar e obter pecas associadas ao pedido.</param>
        /// <param name="userRepository">Repositorio usado para validar o utilizador autenticado que confere a rececao.</param>
        /// <param name="mapper">Mapeador de objetos para conversao entre Dtos e entidades.</param>
        /// <param name="logger">Logger para rastreabilidade das operacoes do servico.</param>
        public PedidoMaterialService(
            IPedidoMaterialRepository pedidoRepository,
            IFornecedorRepository fornecedorRepository,
            IPecaRepository pecaRepository,
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<PedidoMaterialService> logger)
        {
            _pedidoRepository = pedidoRepository;
            _fornecedorRepository = fornecedorRepository;
            _pecaRepository = pecaRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Lista pedidos de material com paginacao.
        /// </summary>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com pedidos de material mapeados para DTO.</returns>
        public async Task<PagedResult<ResponsePedidoMaterialDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var result = await _pedidoRepository.GetPagedWithItensAsync(page, pageSize);
            var mappedItems = _mapper.Map<IEnumerable<ResponsePedidoMaterialDto>>(result.Items);

            return new PagedResult<ResponsePedidoMaterialDto>(
                mappedItems,
                result.TotalCount,
                result.CurrentPage,
                result.PageSize);
        }

        /// <summary>
        /// Obtem um pedido de material pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do pedido.</param>
        /// <returns>DTO do pedido encontrado ou nulo quando nao existe registo.</returns>
        public async Task<ResponsePedidoMaterialDto?> GetByIdAsync(int id)
        {
            var pedido = await _pedidoRepository.GetByIdWithItensAsync(id);
            return pedido == null ? null : _mapper.Map<ResponsePedidoMaterialDto>(pedido);
        }

        /// <summary>
        /// Lista pedidos de material de um fornecedor.
        /// </summary>
        /// <param name="fornecedorId">Identificador do fornecedor.</param>
        /// <param name="page">Numero da pagina solicitada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com pedidos associados ao fornecedor informado.</returns>
        public async Task<PagedResult<ResponsePedidoMaterialDto>> GetByFornecedorIdAsync(int fornecedorId, int page = 1, int pageSize = 10)
        {
            var result = await _pedidoRepository.GetByFornecedorIdWithItensAsync(fornecedorId, page, pageSize);
            var mappedItems = _mapper.Map<IEnumerable<ResponsePedidoMaterialDto>>(result.Items);

            return new PagedResult<ResponsePedidoMaterialDto>(
                mappedItems,
                result.TotalCount,
                result.CurrentPage,
                result.PageSize);
        }

        /// <summary>
        /// Cria um novo pedido de material apos validacao completa do agregado.
        /// </summary>
        /// <remarks>
        /// Fluxo principal:
        /// 1. Valida fornecedor.
        /// 2. Valida existencia de itens.
        /// 3. Rejeita pecas repetidas no mesmo pedido.
        /// 4. Valida existencia de todas as pecas antes de qualquer persistencia.
        /// 5. Persiste o agregado completo numa unica operacao.
        /// </remarks>
        /// <param name="dto">DTO com dados do pedido e das respetivas linhas.</param>
        /// <returns>DTO do pedido criado apos persistencia.</returns>
        public async Task<ResponsePedidoMaterialDto> CreateAsync(CreatePedidoMaterialDto dto)
        {
            _logger.LogInformation(
                "Criacao de pedido de material iniciada para fornecedor {FornecedorId}",
                dto.Fornecedor_id);

            _ = await _fornecedorRepository.GetByIdAsync(dto.Fornecedor_id)
                ?? throw new KeyNotFoundException($"Fornecedor com ID {dto.Fornecedor_id} nao encontrado.");

            if (dto.Itens == null || dto.Itens.Count == 0)
                throw new ArgumentException("Pedido deve conter pelo menos um item.");

            var duplicatePecaIds = dto.Itens
                .GroupBy(i => i.Peca_id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicatePecaIds.Count != 0)
            {
                throw new ArgumentException(
                    $"Pedido contem pecas repetidas: {string.Join(", ", duplicatePecaIds)}.");
            }

            var pecaIds = dto.Itens
                .Select(i => i.Peca_id)
                .Distinct()
                .ToList();

            var pecas = await _pecaRepository.GetByIdsAsync(pecaIds);
            var existingPecaIds = pecas.Items.Select(p => p.Peca_id).ToHashSet();

            var missingPecaIds = pecaIds
                .Where(id => !existingPecaIds.Contains(id))
                .ToList();

            if (missingPecaIds.Count != 0)
            {
                throw new KeyNotFoundException(
                    $"As seguintes pecas nao foram encontradas: {string.Join(", ", missingPecaIds)}.");
            }

            var pedido = _mapper.Map<PedidoMaterial>(dto);
            pedido.DataPedido = DateTime.UtcNow;
            pedido.Estado = EstadoPedido.PENDENTE;

            // Porque: o agregado so e persistido depois de validar todas as dependencias,
            // evitando deixar pedidos ou linhas parciais gravados em caso de falha.
            await _pedidoRepository.AddAsync(pedido);

            var created = await _pedidoRepository.GetByIdWithItensAsync(pedido.PedidoMaterial_id)
                ?? throw new InvalidOperationException(
                    $"Pedido de material {pedido.PedidoMaterial_id} foi criado mas nao pode ser relido.");

            _logger.LogInformation("Pedido de material {PedidoId} criado com sucesso", created.PedidoMaterial_id);

            return _mapper.Map<ResponsePedidoMaterialDto>(created);
        }

        /// <summary>
        /// Regista a rececao de um pedido de material e desbloqueia as pecas para producao.
        /// </summary>
        /// <remarks>
        /// Invariantes desta operacao:
        /// 1. O pedido tem de existir.
        /// 2. O utilizador conferente tem de existir.
        /// 3. O pedido so pode ser recebido uma vez.
        /// 4. Todas as pecas associadas ao pedido passam a ter MaterialRecebido = true.
        /// 5. A atualizacao do pedido e das pecas deve ser persistida de forma consistente.
        /// </remarks>
        /// <param name="pedidoId">Identificador do pedido a marcar como recebido.</param>
        /// <param name="userId">Identificador do utilizador autenticado que confere a rececao.</param>
        /// <returns>Task assincrona concluida apos persistencia consistente.</returns>
        public async Task RegistarRececaoAsync(int pedidoId, int userId)
        {
            _logger.LogInformation(
                "Registo de rececao iniciado para pedido {PedidoId} pelo utilizador autenticado {UserId}",
                pedidoId,
                userId);

            var pedido = await _pedidoRepository.GetByIdWithItensAsync(pedidoId)
                ?? throw new KeyNotFoundException($"Pedido com ID {pedidoId} nao encontrado.");

            _ = await _userRepository.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException($"Utilizador com ID {userId} nao encontrado.");

            // Invariante: a rececao so pode ser registada uma vez para preservar auditoria.
            if (pedido.Estado == EstadoPedido.RECEBIDO)
                throw new BusinessConflictException($"Pedido com ID {pedidoId} ja foi recebido.");

            var pecaIds = pedido.Itens
                .Select(i => i.Peca_id)
                .Distinct()
                .ToList();

            var pecas = (await _pecaRepository.GetByIdsAsync(pecaIds)).Items.ToList();
            var existingPecaIds = pecas.Select(p => p.Peca_id).ToHashSet();

            var missingPecaIds = pecaIds
                .Where(id => !existingPecaIds.Contains(id))
                .ToList();

            if (missingPecaIds.Count != 0)
            {
                throw new KeyNotFoundException(
                    $"As seguintes pecas associadas ao pedido nao foram encontradas: {string.Join(", ", missingPecaIds)}.");
            }

            // Porque: ao registar a rececao, a producao das pecas deixa de estar bloqueada.
            foreach (var peca in pecas)
            {
                peca.MaterialRecebido = true;
            }

            pedido.Estado = EstadoPedido.RECEBIDO;
            pedido.DataRececao = DateTime.UtcNow;
            pedido.UserConferente_id = userId;

            await _pedidoRepository.RegistarRececaoAsync(pedido, pecas);

            _logger.LogInformation(
                "Rececao do pedido {PedidoId} registada com sucesso pelo utilizador {UserId}",
                pedidoId,
                userId);
        }

        /// <summary>
        /// Remove um pedido de material pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do pedido a remover.</param>
        /// <returns>Task assincrona concluida apos remocao do pedido.</returns>
        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Eliminacao de pedido de material iniciada {PedidoId}", id);

            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null)
                throw new KeyNotFoundException($"Pedido com ID {id} nao encontrado.");

            await _pedidoRepository.DeleteAsync(id);

            _logger.LogInformation("Pedido de material {PedidoId} eliminado com sucesso", id);
        }
    }
}
