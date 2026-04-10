using Microsoft.Extensions.Logging;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Application.Interface.Comercio.IEncomenda;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;

namespace TipMolde.Infrastructure.Service
{
    public class EncomendaService : IEncomendaService
    {
        private readonly IEncomendaRepository _encomendaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly ILogger<EncomendaService> _logger;
        public EncomendaService(
            IEncomendaRepository encomendaRepository, 
            IClienteRepository clienteRepository,
            ILogger<EncomendaService> logger)
        {
            _encomendaRepository = encomendaRepository;
            _clienteRepository = clienteRepository;
            _logger = logger;
        }
        public Task<PagedResult<Encomenda>> GetAllAsync(int page = 1, int pageSize = 10) =>
            _encomendaRepository.GetAllAsync(page, pageSize);

        public Task<Encomenda?> GetByIdAsync(int id)
        {
            return _encomendaRepository.GetByIdAsync(id);
        }

        public Task<Encomenda?> GetEncomendaWithMoldesAsync(int id)
        {
            return _encomendaRepository.GetWithMoldesAsync(id);
        }

        public Task<IEnumerable<Encomenda>> GetByEstadoAsync(EstadoEncomenda estado)
        {
            return _encomendaRepository.GetByEstadoAsync(estado);
        }

        public Task<IEnumerable<Encomenda>> GetEncomendasPorConcluirAsync()
        {
            return _encomendaRepository.GetEncomendasPorConcluirAsync();
        }

        public Task<Encomenda?> GetByNumeroEncomendaClienteAsync(string numero)
        {
            return _encomendaRepository.GetByNumeroEncomendaClienteAsync(numero);
        }

        public async Task<Encomenda> CreateAsync(Encomenda encomenda)
        {
            _logger.LogInformation("Criacao de encomenda iniciada para cliente {ClienteId} com numero {Numero}",
                encomenda.Cliente_id, encomenda.NumeroEncomendaCliente);
            if (string.IsNullOrWhiteSpace(encomenda.NumeroEncomendaCliente))
                throw new ArgumentException("O numero de encomenda do cliente e obrigatorio.");

            var cliente = await _clienteRepository.GetByIdAsync(encomenda.Cliente_id);
            if (cliente == null)
            {
                _logger.LogWarning("Criacao de encomenda falhou: cliente nao encontrado {ClienteId}", encomenda.Cliente_id);
                throw new KeyNotFoundException($"Cliente com ID {encomenda.Cliente_id} nao encontrado.");
            }

            var existente = await _encomendaRepository
                .GetByNumeroEncomendaClienteAsync(encomenda.NumeroEncomendaCliente);
            if (existente != null)
            {
                _logger.LogWarning("Criacao de encomenda falhou: numero duplicado {Numero}", encomenda.NumeroEncomendaCliente);
                throw new ArgumentException($"Ja existe uma encomenda com o numero '{encomenda.NumeroEncomendaCliente}'.");
            }

            encomenda.Estado = EstadoEncomenda.CONFIRMADA;
            encomenda.DataRegisto = DateTime.UtcNow;

            await _encomendaRepository.AddAsync(encomenda);
            _logger.LogInformation("Encomenda criada com sucesso {EncomendaId} estado {Estado}",
                encomenda.Encomenda_id, encomenda.Estado);
            return encomenda;
        }

        public async Task UpdateAsync(Encomenda encomenda)
        {
            _logger.LogInformation("Atualizacao de encomenda iniciada {EncomendaId}", encomenda.Encomenda_id);
            var existente = await _encomendaRepository.GetByIdAsync(encomenda.Encomenda_id);
            if (existente == null)
            {
                _logger.LogWarning("Atualizacao de encomenda falhou: nao encontrada {EncomendaId}", encomenda.Encomenda_id);
                throw new KeyNotFoundException($"Encomenda com ID {encomenda.Encomenda_id} nao encontrada.");
            }

            existente.NumeroEncomendaCliente = string.IsNullOrWhiteSpace(encomenda.NumeroEncomendaCliente)
                ? existente.NumeroEncomendaCliente
                : encomenda.NumeroEncomendaCliente.Trim();

            existente.NumeroProjetoCliente = encomenda.NumeroProjetoCliente ?? existente.NumeroProjetoCliente;
            existente.NomeServicoCliente = encomenda.NomeServicoCliente ?? existente.NomeServicoCliente;
            existente.NomeResponsavelCliente = encomenda.NomeResponsavelCliente ?? existente.NomeResponsavelCliente;

            await _encomendaRepository.UpdateAsync(existente);
            _logger.LogInformation("Encomenda atualizada com sucesso {EncomendaId}", encomenda.Encomenda_id);
        }

        public async Task UpdateEstadoAsync(int id, EstadoEncomenda novoEstado)
        {
           var encomenda = await _encomendaRepository.GetByIdAsync(id);
            if (encomenda == null) 
            {
                _logger.LogInformation("Alteracao de estado da encomenda {EncomendaId}: {EstadoAtual} -> {NovoEstado}",
                    id, encomenda.Estado, novoEstado);
                throw new KeyNotFoundException($"Encomenda com ID {id} nao encontrada.");
            }

            ValidarTransicaoEstado(encomenda.Estado, novoEstado);
            encomenda.Estado = novoEstado;
            await _encomendaRepository.UpdateAsync(encomenda);
            _logger.LogInformation("Estado da encomenda atualizado {EncomendaId} para {NovoEstado}", id, novoEstado);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Eliminacao de encomenda iniciada {EncomendaId}", id);
            var encomenda = await _encomendaRepository.GetByIdAsync(id);
            if (encomenda == null)
            {
                _logger.LogWarning("Eliminacao de encomenda falhou: nao encontrada {EncomendaId}", id);
                throw new KeyNotFoundException($"Encomenda com ID {id} nao encontrada.");
            }

            await _encomendaRepository.DeleteAsync(id);
            _logger.LogInformation("Encomenda eliminada com sucesso {EncomendaId}", id);
        }

        private static void ValidarTransicaoEstado(EstadoEncomenda estadoAtual, EstadoEncomenda novoEstado)
        {
            var transicoesValidas = new Dictionary<EstadoEncomenda, List<EstadoEncomenda>>
            {
                { EstadoEncomenda.CONFIRMADA,             new() { EstadoEncomenda.EM_PRODUCAO, EstadoEncomenda.CANCELADA } },
                { EstadoEncomenda.EM_PRODUCAO,            new() { EstadoEncomenda.PARCIALMENTE_ENTREGUE, EstadoEncomenda.CONCLUIDA, EstadoEncomenda.CANCELADA } },
                { EstadoEncomenda.PARCIALMENTE_ENTREGUE,  new() { EstadoEncomenda.CONCLUIDA } },
                { EstadoEncomenda.CONCLUIDA,              new() },
                { EstadoEncomenda.CANCELADA,              new() }
            };

            if (!transicoesValidas[estadoAtual].Contains(novoEstado))
                throw new ArgumentException(
                    $"Transição de estado inválida: não é possível passar de {estadoAtual} para {novoEstado}.");
        }
    }
}
