using Microsoft.Extensions.Logging;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.IFornecedor;
using TipMolde.Application.Interface.Comercio.IPedidoMaterial;
using TipMolde.Application.Interface.Comercio.IPedidoMaterial.IItemPedidoMaterial;
using TipMolde.Application.Interface.Producao.IPeca;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;
namespace TipMolde.Application.Service
{
    public class PedidoMaterialService : IPedidoMaterialService
    {
        private readonly IPedidoMaterialRepository _pedidoRepository;
        private readonly IItemPedidoMaterialRepository _itemRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IPecaRepository _pecaRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PedidoMaterialService> _logger;

        public PedidoMaterialService(
            IPedidoMaterialRepository pedidoRepository,
            IItemPedidoMaterialRepository itemRepository,
            IFornecedorRepository fornecedorRepository,
            IPecaRepository pecaRepository,
            IUserRepository userRepository,
            ILogger<PedidoMaterialService> logger)
        {
            _pedidoRepository = pedidoRepository;
            _itemRepository = itemRepository;
            _fornecedorRepository = fornecedorRepository;
            _pecaRepository = pecaRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public Task<PagedResult<PedidoMaterial>> GetAllAsync(int page = 1, int pageSize = 10) =>
            _pedidoRepository.GetAllAsync(page, pageSize);

        public Task<PedidoMaterial?> GetByIdAsync(int id) => _pedidoRepository.GetByIdAsync(id);

        public Task<PedidoMaterial?> GetWithItensAsync(int id) => _pedidoRepository.GetWithItensAsync(id);

        public Task<IEnumerable<PedidoMaterial>> GetByFornecedorIdAsync(int fornecedorId) =>
            _pedidoRepository.GetByFornecedorIdAsync(fornecedorId);

        public async Task<PedidoMaterial> CreateAsync(PedidoMaterial pedido, IEnumerable<ItemPedidoMaterial> itens)
        {
            _logger.LogInformation("Criacao de pedido material iniciada para fornecedor {FornecedorId}", pedido.Fornecedor_id);
            var fornecedor = await _fornecedorRepository.GetByIdAsync(pedido.Fornecedor_id);
            if (fornecedor == null)
            {
                _logger.LogWarning("Criacao de pedido material falhou: fornecedor nao encontrado {FornecedorId}", pedido.Fornecedor_id);
                throw new KeyNotFoundException($"Fornecedor com ID {pedido.Fornecedor_id} nao encontrado.");
            }

            if (itens == null || !itens.Any())
            {
                _logger.LogWarning("Criacao de pedido material falhou: pedido sem itens para fornecedor {FornecedorId}", pedido.Fornecedor_id);
                throw new ArgumentException("Pedido deve conter itens.");
            }
            pedido.DataPedido = DateTime.UtcNow;
            pedido.Estado = EstadoPedido.PENDENTE;

            await _pedidoRepository.AddAsync(pedido);

            foreach (var item in itens)
            {
                var peca = await _pecaRepository.GetByIdAsync(item.Peca_id);
                if (peca == null)
                {
                    _logger.LogWarning("Criacao de pedido material falhou: peca nao encontrada {PecaId}", item.Peca_id);
                    throw new KeyNotFoundException($"Peca com ID {item.Peca_id} nao encontrada.");
                }

                item.PedidoMaterial_id = pedido.PedidoMaterial_id;
                await _itemRepository.AddAsync(item);
            }
            _logger.LogInformation("Pedido material criado com sucesso {PedidoId}", pedido.PedidoMaterial_id);
            return pedido;
        }

        public async Task RegistarRececaoAsync(int pedidoId, int userId)
        {
            _logger.LogInformation("Registo de rececao iniciado para pedido {PedidoId} por utilizador {UserId}", pedidoId, userId);
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null)
            {
                _logger.LogWarning("Registo de rececao falhou: pedido nao encontrado {PedidoId}", pedidoId);
                throw new KeyNotFoundException($"Pedido com ID {pedidoId} nao encontrado.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Registo de rececao falhou: utilizador nao encontrado {UserId}", userId);
                throw new KeyNotFoundException($"Utilizador com ID {userId} nao encontrado.");
            }


            pedido.Estado = EstadoPedido.RECEBIDO;
            pedido.DataRececao = DateTime.UtcNow;
            pedido.UserConferente_id = userId;

            await _pedidoRepository.UpdateAsync(pedido);
            _logger.LogInformation("Rececao registada com sucesso para pedido {PedidoId} por utilizador {UserId}", pedidoId, userId);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Eliminacao de pedido material iniciada {PedidoId}", id);
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null)
            {
                _logger.LogWarning("Eliminacao de pedido material falhou: pedido nao encontrado {PedidoId}", id);
                throw new KeyNotFoundException($"Pedido com ID {id} nao encontrado.");
            }

            await _pedidoRepository.DeleteAsync(id);
            _logger.LogInformation("Pedido material eliminado com sucesso {PedidoId}", id);
        }
    }
}
