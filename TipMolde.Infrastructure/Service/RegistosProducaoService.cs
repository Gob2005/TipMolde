using TipMolde.Core.Enums;
using TipMolde.Core.Interface.IFases_producao;
using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Interface.IPeca;
using TipMolde.Core.Interface.IRegistosProducao;
using TipMolde.Core.Interface.IUser;
using TipMolde.Core.Models;

namespace TipMolde.Infrastructure.Service
{
    public class RegistosProducaoService : IRegistosProducaoService
    {
        private readonly IRegistosProducaoRepository _rpRepository;
        private readonly IFasesProducaoRepository _fpRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPecaRepository _pecaRepository;

        public RegistosProducaoService(
            IRegistosProducaoRepository rpRepository,
            IFasesProducaoRepository fpRepository,
            IUserRepository userRepository,
            IPecaRepository pecaRepository)
        {
            _rpRepository = rpRepository;
            _fpRepository = fpRepository;
            _userRepository = userRepository;
            _pecaRepository = pecaRepository;
        }

        public Task<IEnumerable<RegistosProducao>> GetAllRegistosProducaoAsync()
        {
            return _rpRepository.GetAllAsync();
        }

        public Task<RegistosProducao?> GetRegistoProducaoByIdAsync(int id)
        {
            return _rpRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<RegistosProducao>> GetHistoricoAsync(int faseId, int pecaId)
        {
            return _rpRepository.GetHistoricoAsync(faseId, pecaId);
        }

        public Task<RegistosProducao?> GetUltimoRegistoAsync(int faseId, int pecaId)
        {
            return _rpRepository.GetUltimoRegistoAsync(faseId, pecaId);
        }
        public async Task<RegistosProducao> CreateRegistoProducaoAsync(RegistosProducao registo)
        {
            var fase = await _fpRepository.GetByIdAsync(registo.Fase_id);
            if (fase == null)
                throw new KeyNotFoundException($"Fase com ID {registo.Fase_id} nao encontrada.");

            var operador = await _userRepository.GetByIdAsync(registo.Operador_id);
            if (operador == null)
                throw new KeyNotFoundException($"Operador com ID {registo.Operador_id} nao encontrado.");

            var peca = await _pecaRepository.GetByIdAsync(registo.Peca_id);
            if (peca == null)
                throw new KeyNotFoundException($"Peca com ID {registo.Peca_id} nao encontrada.");

            var ultimoRegisto = await _rpRepository.GetUltimoRegistoAsync(
                registo.Fase_id, registo.Peca_id);

            var estadoActual = ultimoRegisto?.Estado_producao ?? EstadoProducao.PENDENTE;
            ValidarTransicaoEstado(estadoActual, registo.Estado_producao);

            registo.Data_hora = DateTime.UtcNow;

            await _rpRepository.AddAsync(registo);
            return registo;
        }

        public async Task DeleteRegistoProducaoAsync(int id)
        {
            var registo = await _rpRepository.GetByIdAsync(id);
            if (registo == null)
                throw new KeyNotFoundException($"Registo de Producao com ID {id} nao encontrado.");

            await _rpRepository.DeleteAsync(id);
        }
        private static void ValidarTransicaoEstado(EstadoProducao estadoActual, EstadoProducao novoEstado)
        {
            var transicoesValidas = new Dictionary<EstadoProducao, List<EstadoProducao>>
            {
                { EstadoProducao.PENDENTE, new() { EstadoProducao.PREPARACAO } },
                { EstadoProducao.PREPARACAO, new() { EstadoProducao.EM_CURSO } },
                { EstadoProducao.EM_CURSO,   new() { EstadoProducao.PAUSADO, EstadoProducao.CONCLUIDO } },
                { EstadoProducao.PAUSADO,    new() { EstadoProducao.EM_CURSO, EstadoProducao.PREPARACAO } },
                { EstadoProducao.CONCLUIDO,  new() }
            };

            if (!transicoesValidas[estadoActual].Contains(novoEstado))
                throw new ArgumentException(
                    $"Transicao de estado invalida: nao e possivel passar de {estadoActual} para {novoEstado}.");
        }
    }
}
