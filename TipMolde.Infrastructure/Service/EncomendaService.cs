using TipMolde.Core.Enums;
using TipMolde.Core.Interface.Comercio.ICliente;
using TipMolde.Core.Interface.Comercio.IEncomenda;
using TipMolde.Core.Models.Comercio;

namespace TipMolde.Infrastructure.Service
{
    public class EncomendaService : IEncomendaService
    {
        private readonly IEncomendaRepository _encomendaRepository;
        private readonly IClienteRepository _clienteRepository;
        public EncomendaService(
            IEncomendaRepository encomendaRepository, 
            IClienteRepository clienteRepository)
        {
            _encomendaRepository = encomendaRepository;
            _clienteRepository = clienteRepository;
        }
        public Task<IEnumerable<Encomenda>> GetAllEncomendasAsync()
        {
            return _encomendaRepository.GetAllAsync();
        }

        public Task<Encomenda?> GetEncomendaByIdAsync(int id)
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

        public async Task<Encomenda> CreateEncomendaAsync(Encomenda encomenda)
        {
            if (string.IsNullOrWhiteSpace(encomenda.NumeroEncomendaCliente))
                throw new ArgumentException("O numero de encomenda do cliente e obrigatorio.");

            var cliente = await _clienteRepository.GetByIdAsync(encomenda.Cliente_id);
            if (cliente == null)
                throw new KeyNotFoundException($"Cliente com ID {encomenda.Cliente_id} nao encontrado.");

            var existente = await _encomendaRepository
                .GetByNumeroEncomendaClienteAsync(encomenda.NumeroEncomendaCliente);
            if (existente != null)
                throw new ArgumentException($"Ja existe uma encomenda com o numero '{encomenda.NumeroEncomendaCliente}'.");

            encomenda.Estado = EstadoEncomenda.CONFIRMADA;
            encomenda.DataRegisto = DateTime.UtcNow;

            await _encomendaRepository.AddAsync(encomenda);
            return encomenda;
        }

        public async Task UpdateEncomendaAsync(Encomenda encomenda)
        {
            var existente = await _encomendaRepository.GetByIdAsync(encomenda.Encomenda_id);
            if (existente == null)
                throw new KeyNotFoundException($"Encomenda com ID {encomenda.Encomenda_id} nao encontrada.");

            existente.NumeroEncomendaCliente = string.IsNullOrWhiteSpace(encomenda.NumeroEncomendaCliente)
                ? existente.NumeroEncomendaCliente
                : encomenda.NumeroEncomendaCliente.Trim();

            existente.NumeroProjetoCliente = encomenda.NumeroProjetoCliente ?? existente.NumeroProjetoCliente;
            existente.NomeServicoCliente = encomenda.NomeServicoCliente ?? existente.NomeServicoCliente;
            existente.NomeResponsavelCliente = encomenda.NomeResponsavelCliente ?? existente.NomeResponsavelCliente;

            await _encomendaRepository.UpdateAsync(existente);
        }

        public async Task UpdateEstadoEncomendaAsync(int id, EstadoEncomenda novoEstado)
        {
            var encomenda = await _encomendaRepository.GetByIdAsync(id);
            if (encomenda == null)
                throw new KeyNotFoundException($"Encomenda com ID {id} nao encontrada.");

            ValidarTransicaoEstado(encomenda.Estado, novoEstado);
            encomenda.Estado = novoEstado;
            await _encomendaRepository.UpdateAsync(encomenda);
        }

        public async Task DeleteEncomendaAsync(int id)
        {
            var encomenda = await _encomendaRepository.GetByIdAsync(id);
            if (encomenda == null)
                throw new KeyNotFoundException($"Encomenda com ID {id} nao encontrada.");

            await _encomendaRepository.DeleteAsync(id);
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
