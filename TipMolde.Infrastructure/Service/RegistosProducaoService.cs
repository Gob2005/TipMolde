using TipMolde.Core.Enums;
using TipMolde.Core.Interface.Producao.IFasesProducao;
using TipMolde.Core.Interface.Producao.IMaquina;
using TipMolde.Core.Interface.Producao.IMolde;
using TipMolde.Core.Interface.Producao.IPeca;
using TipMolde.Core.Interface.Producao.IRegistosProducao;
using TipMolde.Core.Interface.Utilizador.IUser;
using TipMolde.Core.Models.Producao;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Infrastructure.Service
{
    public class RegistosProducaoService : IRegistosProducaoService
    {
        private readonly IRegistosProducaoRepository _rpRepository;
        private readonly IFasesProducaoRepository _fpRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPecaRepository _pecaRepository;
        private readonly IMaquinaRepository _maquinaRepository;

        public RegistosProducaoService(
            IRegistosProducaoRepository rpRepository,
            IFasesProducaoRepository fpRepository,
            IUserRepository userRepository,
            IMaquinaRepository maquinaRepository,
            IPecaRepository pecaRepository)
        {
            _rpRepository = rpRepository;
            _fpRepository = fpRepository;
            _userRepository = userRepository;
            _maquinaRepository = maquinaRepository;
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
            var fase = await _fpRepository.GetByIdAsync(registo.Fase_id)
                ?? throw new KeyNotFoundException($"Fase com ID {registo.Fase_id} nao encontrada.");

            var operador = await _userRepository.GetByIdAsync(registo.Operador_id)
                ?? throw new KeyNotFoundException($"Operador com ID {registo.Operador_id} nao encontrado.");

            var peca = await _pecaRepository.GetByIdAsync(registo.Peca_id)
                ?? throw new KeyNotFoundException($"Peca com ID {registo.Peca_id} nao encontrada.");

            if (!peca.MaterialRecebido)
                throw new ArgumentException("Nao e possivel iniciar producao sem material recebido.");

            var ultimo = await _rpRepository.GetUltimoRegistoAsync(registo.Fase_id, registo.Peca_id);
            var estadoAtual = ultimo?.Estado_producao;
            ValidarTransicaoEstado(estadoAtual, registo.Estado_producao);

            if ((registo.Estado_producao == EstadoProducao.PREPARACAO || registo.Estado_producao == EstadoProducao.EM_CURSO) &&
                registo.Maquina_id.HasValue)
            {
                var maquina = await _maquinaRepository.GetByIdUnicoAsync(registo.Maquina_id.Value);
                if (maquina.FaseDedicada_id != registo.Fase_id)
                    throw new ArgumentException("Maquina nao esta apta para esta fase.");

                if (maquina != null && maquina.Estado == EstadoMaquina.DISPONIVEL)
                {
                    maquina.Estado = EstadoMaquina.EM_USO;
                    await _maquinaRepository.UpdateAsync(maquina);
                } else
                {
                    throw new ArgumentException("Maquina indisponivel.");
                }
            }

            if ((registo.Estado_producao == EstadoProducao.PAUSADO || registo.Estado_producao == EstadoProducao.CONCLUIDO) &&
                registo.Maquina_id.HasValue)
            {
                var maquina = await _maquinaRepository.GetByIdUnicoAsync(registo.Maquina_id.Value);
                if (maquina != null && maquina.Estado == EstadoMaquina.EM_USO)
                {
                    maquina.Estado = EstadoMaquina.DISPONIVEL;
                    await _maquinaRepository.UpdateAsync(maquina);
                }
            }

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
        private static void ValidarTransicaoEstado(EstadoProducao? estadoActual, EstadoProducao novoEstado)
        {
            if (estadoActual is null)
            {
                if (novoEstado != EstadoProducao.PREPARACAO)
                    throw new ArgumentException("Primeiro estado deve ser PREPARACAO.");
                return;
            }

            var transicoesValidas = new Dictionary<EstadoProducao, List<EstadoProducao>>
            {
                { EstadoProducao.PENDENTE, new() { EstadoProducao.PREPARACAO } },
                { EstadoProducao.PREPARACAO, new() { EstadoProducao.EM_CURSO } },
                { EstadoProducao.EM_CURSO,   new() { EstadoProducao.PAUSADO, EstadoProducao.CONCLUIDO } },
                { EstadoProducao.PAUSADO,    new() { EstadoProducao.EM_CURSO, EstadoProducao.PREPARACAO } },
                { EstadoProducao.CONCLUIDO,  new() }
            };

            if (!transicoesValidas[estadoActual.Value].Contains(novoEstado))
                throw new ArgumentException(
                    $"Transicao de estado invalida: nao e possivel passar de {estadoActual} para {novoEstado}.");
        }
    }
}
