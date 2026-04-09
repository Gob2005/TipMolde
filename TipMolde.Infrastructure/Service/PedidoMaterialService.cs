using TipMolde.Core.Enums;
using TipMolde.Core.Interface.Comercio.IFornecedor;
using TipMolde.Core.Interface.Comercio.IPedidoMaterial;
using TipMolde.Core.Interface.Comercio.IPedidoMaterial.IItemPedidoMaterial;
using TipMolde.Core.Interface.Producao.IPeca;
using TipMolde.Core.Interface.Utilizador.IUser;
using TipMolde.Core.Models.Comercio;

namespace TipMolde.Infrastructure.Service
{
    public class PedidoMaterialService : IPedidoMaterialService
    {
        private readonly IPedidoMaterialRepository _pedidoRepository;
        private readonly IItemPedidoMaterialRepository _itemRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IPecaRepository _pecaRepository;
        private readonly IUserRepository _userRepository;

        public PedidoMaterialService(
            IPedidoMaterialRepository pedidoRepository,
            IItemPedidoMaterialRepository itemRepository,
            IFornecedorRepository fornecedorRepository,
            IPecaRepository pecaRepository,
            IUserRepository userRepository)
        {
            _pedidoRepository = pedidoRepository;
            _itemRepository = itemRepository;
            _fornecedorRepository = fornecedorRepository;
            _pecaRepository = pecaRepository;
            _userRepository = userRepository;
        }

        public Task<IEnumerable<PedidoMaterial>> GetAllAsync() => _pedidoRepository.GetAllAsync();

        public Task<PedidoMaterial?> GetByIdAsync(int id) => _pedidoRepository.GetByIdAsync(id);

        public Task<PedidoMaterial?> GetWithItensAsync(int id) => _pedidoRepository.GetWithItensAsync(id);

        public Task<IEnumerable<PedidoMaterial>> GetByFornecedorIdAsync(int fornecedorId) =>
            _pedidoRepository.GetByFornecedorIdAsync(fornecedorId);

        public async Task<PedidoMaterial> CreateAsync(PedidoMaterial pedido, IEnumerable<ItemPedidoMaterial> itens)
        {
            var fornecedor = await _fornecedorRepository.GetByIdAsync(pedido.Fornecedor_id);
            if (fornecedor == null)
                throw new KeyNotFoundException($"Fornecedor com ID {pedido.Fornecedor_id} nao encontrado.");

            if (itens == null || !itens.Any())
                throw new ArgumentException("Pedido deve conter itens.");

            pedido.DataPedido = DateTime.UtcNow;
            pedido.Estado = EstadoPedido.PENDENTE;

            await _pedidoRepository.AddAsync(pedido);

            foreach (var item in itens)
            {
                var peca = await _pecaRepository.GetByIdAsync(item.Peca_id);
                if (peca == null)
                    throw new KeyNotFoundException($"Peca com ID {item.Peca_id} nao encontrada.");

                item.PedidoMaterial_id = pedido.PedidoMaterial_id;
                await _itemRepository.AddAsync(item);
            }

            return pedido;
        }

        public async Task RegistarRececaoAsync(int pedidoId, int userId)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null)
                throw new KeyNotFoundException($"Pedido com ID {pedidoId} nao encontrado.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Utilizador com ID {userId} nao encontrado.");

            pedido.Estado = EstadoPedido.RECEBIDO;
            pedido.DataRececao = DateTime.UtcNow;
            pedido.UserConferente_id = userId;

            await _pedidoRepository.UpdateAsync(pedido);
        }

        public async Task DeleteAsync(int id)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null)
                throw new KeyNotFoundException($"Pedido com ID {id} nao encontrado.");

            await _pedidoRepository.DeleteAsync(id);
        }
    }
}
