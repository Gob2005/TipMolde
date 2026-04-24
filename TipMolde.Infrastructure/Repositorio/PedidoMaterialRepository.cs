using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.IPedidoMaterial;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa operacoes de persistencia especificas do agregado PedidoMaterial.
    /// </summary>
    /// <remarks>
    /// As leituras desta classe carregam as linhas do pedido para manter o contrato HTTP coerente
    /// sem incluir navegacoes desnecessarias para a entidade Peca.
    /// </remarks>
    public class PedidoMaterialRepository : GenericRepository<PedidoMaterial, int>, IPedidoMaterialRepository
    {
        /// <summary>
        /// Construtor de PedidoMaterialRepository.
        /// </summary>
        /// <param name="context">Contexto EF Core da aplicacao.</param>
        public PedidoMaterialRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lista pedidos de material com paginacao, incluindo as respetivas linhas.
        /// </summary>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com pedidos enriquecidos com itens.</returns>
        public async Task<PagedResult<PedidoMaterial>> GetPagedWithItensAsync(int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;

            var query = _context.PedidosMaterial
                .AsNoTracking()
                .Include(p => p.Itens);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(p => p.DataPedido)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<PedidoMaterial>(items, totalCount, page, pageSize);
        }

        /// <summary>
        /// Obtem um pedido de material pelo identificador, incluindo as respetivas linhas.
        /// </summary>
        /// <param name="id">Identificador unico do pedido.</param>
        /// <returns>Pedido encontrado ou nulo quando nao existe registo.</returns>
        public Task<PedidoMaterial?> GetByIdWithItensAsync(int id)
        {
            return _context.PedidosMaterial
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.PedidoMaterial_id == id);
        }

        /// <summary>
        /// Lista pedidos de material de um fornecedor, incluindo as respetivas linhas.
        /// </summary>
        /// <param name="fornecedorId">Identificador do fornecedor.</param>
        /// <param name="page">Numero da pagina solicitada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com pedidos associados ao fornecedor.</returns>
        public async Task<PagedResult<PedidoMaterial>> GetByFornecedorIdWithItensAsync(int fornecedorId, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;

            var query = _context.PedidosMaterial
                .AsNoTracking()
                .Include(p => p.Itens)
                .Where(p => p.Fornecedor_id == fornecedorId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.DataPedido)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<PedidoMaterial>(items, totalCount, page, pageSize);
        }

        /// <summary>
        /// Persiste de forma consistente a rececao do pedido e o desbloqueio das pecas associadas.
        /// </summary>
        /// <param name="pedido">Pedido ja atualizado com estado, data e utilizador conferente.</param>
        /// <param name="pecas">Colecao de pecas a marcar com material recebido.</param>
        /// <returns>Task assincrona concluida apos commit atomico.</returns>
        public async Task RegistarRececaoAsync(PedidoMaterial pedido, IEnumerable<Peca> pecas)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Pecas.UpdateRange(pecas);
                _context.PedidosMaterial.Update(pedido);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
