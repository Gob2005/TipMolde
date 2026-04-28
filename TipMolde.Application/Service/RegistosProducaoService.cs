using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Application.Interface.Producao.IMaquina;
using TipMolde.Application.Interface.Producao.IPeca;
using TipMolde.Application.Interface.Producao.IRegistosProducao;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Service
{
    /// <summary>
    /// Implementa os casos de uso de registos de producao.
    /// </summary>
    /// <remarks>
    /// Centraliza validacoes de fase, operador, peca, maquina e transicoes de estado
    /// antes de persistir eventos de producao.
    /// </remarks>
    public class RegistosProducaoService : IRegistosProducaoService
    {
        private readonly IRegistosProducaoRepository _rpRepository;
        private readonly IFasesProducaoRepository _fpRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPecaRepository _pecaRepository;
        private readonly IMaquinaRepository _maquinaRepository;

        /// <summary>
        /// Construtor de RegistosProducaoService.
        /// </summary>
        /// <param name="rpRepository">Repositorio responsavel pelos registos de producao.</param>
        /// <param name="fpRepository">Repositorio usado para validar fases de producao.</param>
        /// <param name="userRepository">Repositorio usado para validar operadores.</param>
        /// <param name="maquinaRepository">Repositorio usado para validar e atualizar maquinas.</param>
        /// <param name="pecaRepository">Repositorio usado para validar pecas em producao.</param>
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

        /// <summary>
        /// Lista registos de producao com paginacao.
        /// </summary>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com registos de producao.</returns>
        public Task<PagedResult<RegistosProducao>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var (normalizedPage, normalizedPageSize) = PaginationDefaults.Normalize(page, pageSize);
            return _rpRepository.GetAllAsync(normalizedPage, normalizedPageSize);
        }

        /// <summary>
        /// Obtem um registo de producao pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do registo.</param>
        /// <returns>Registo encontrado ou nulo quando nao existe.</returns>
        public Task<RegistosProducao?> GetByIdAsync(int id)
        {
            return _rpRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Obtem o historico de producao de uma peca numa fase.
        /// </summary>
        /// <param name="faseId">Identificador da fase de producao.</param>
        /// <param name="pecaId">Identificador da peca.</param>
        /// <returns>Colecao de registos historicos encontrados.</returns>
        public Task<IEnumerable<RegistosProducao>> GetHistoricoAsync(int faseId, int pecaId)
        {
            return _rpRepository.GetHistoricoAsync(faseId, pecaId);
        }

        /// <summary>
        /// Obtem o ultimo registo de producao de uma peca numa fase.
        /// </summary>
        /// <param name="faseId">Identificador da fase de producao.</param>
        /// <param name="pecaId">Identificador da peca.</param>
        /// <returns>Ultimo registo encontrado ou nulo quando ainda nao existe historico.</returns>
        public Task<RegistosProducao?> GetUltimoRegistoAsync(int faseId, int pecaId)
        {
            return _rpRepository.GetUltimoRegistoAsync(faseId, pecaId);
        }

        /// <summary>
        /// Cria um novo registo de producao apos validar dependencias e transicao de estado.
        /// </summary>
        /// <remarks>
        /// Fluxo principal:
        /// 1. Valida fase, operador e peca.
        /// 2. Garante que a peca ja tem material recebido.
        /// 3. Valida transicao de estado face ao ultimo registo.
        /// 4. Atualiza estado da maquina quando aplicavel.
        /// 5. Persiste o registo com data gerada no servidor.
        /// </remarks>
        /// <param name="registo">Entidade com dados do evento de producao.</param>
        /// <returns>Registo persistido.</returns>
        public async Task<RegistosProducao> CreateAsync(RegistosProducao registo)
        {
            if(await _fpRepository.GetByIdAsync(registo.Fase_id) == null)
                throw new KeyNotFoundException($"Fase com ID {registo.Fase_id} nao encontrada.");

            if(await _userRepository.GetByIdAsync(registo.Operador_id) == null)
                throw new KeyNotFoundException($"Operador com ID {registo.Operador_id} nao encontrado.");

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
                var maquina = await _maquinaRepository.GetByIdUnicoAsync(registo.Maquina_id.Value)
                    ?? throw new KeyNotFoundException($"Maquina com ID {registo.Maquina_id.Value} nao encontrada.");
                if (maquina.FaseDedicada_id != registo.Fase_id)
                    throw new ArgumentException("Maquina nao esta apta para esta fase.");

                if (maquina.Estado == EstadoMaquina.DISPONIVEL)
                {
                    maquina.Estado = EstadoMaquina.EM_USO;
                    await _maquinaRepository.UpdateAsync(maquina);
                }
                else
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

        /// <summary>
        /// Valida se a transicao de estado de producao e permitida.
        /// </summary>
        /// <param name="estadoActual">Estado persistido no ultimo registo, quando existe.</param>
        /// <param name="novoEstado">Novo estado solicitado.</param>
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
