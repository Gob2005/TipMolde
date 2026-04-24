using AutoMapper;
using Microsoft.Extensions.Logging;
using TipMolde.Application.Dtos.PecaDto;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Application.Interface.Producao.IPeca;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Service
{
    /// <summary>
    /// Implementa os casos de uso da feature Peca.
    /// </summary>
    /// <remarks>
    /// Centraliza validacoes de negocio, atualizacao parcial e orquestracao funcional
    /// da criacao, consulta, edicao e remocao de pecas associadas a um molde.
    /// </remarks>
    public class PecaService : IPecaService
    {
        private readonly IPecaRepository _pecaRepository;
        private readonly IMoldeRepository _moldeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PecaService> _logger;

        /// <summary>
        /// Construtor de PecaService.
        /// </summary>
        /// <param name="pecaRepository">Repositorio do agregado Peca.</param>
        /// <param name="moldeRepository">Repositorio usado para validar o molde associado.</param>
        /// <param name="mapper">Mapper para conversao entre Dtos e entidade.</param>
        /// <param name="logger">Logger para rastreabilidade das operacoes criticas.</param>
        public PecaService(
            IPecaRepository pecaRepository,
            IMoldeRepository moldeRepository,
            IMapper mapper,
            ILogger<PecaService> logger)
        {
            _pecaRepository = pecaRepository;
            _moldeRepository = moldeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Lista pecas de forma paginada.
        /// </summary>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com Dtos de peca.</returns>
        public async Task<PagedResult<ResponsePecaDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var result = await _pecaRepository.GetAllAsync(page, pageSize);
            var items = _mapper.Map<IEnumerable<ResponsePecaDto>>(result.Items);
            return new PagedResult<ResponsePecaDto>(items, result.TotalCount, result.CurrentPage, result.PageSize);
        }

        /// <summary>
        /// Obtem uma peca por identificador.
        /// </summary>
        /// <param name="id">Identificador interno da peca.</param>
        /// <returns>DTO da peca quando encontrada; nulo caso contrario.</returns>
        public async Task<ResponsePecaDto?> GetByIdAsync(int id)
        {
            var peca = await _pecaRepository.GetByIdAsync(id);
            return peca == null ? null : _mapper.Map<ResponsePecaDto>(peca);
        }

        /// <summary>
        /// Lista pecas associadas a um molde.
        /// </summary>
        /// <param name="moldeId">Identificador do molde.</param>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com Dtos de peca.</returns>
        public async Task<PagedResult<ResponsePecaDto>> GetByMoldeIdAsync(int moldeId, int page = 1, int pageSize = 10)
        {
            var result = await _pecaRepository.GetByMoldeIdAsync(moldeId, page, pageSize);
            var items = _mapper.Map<IEnumerable<ResponsePecaDto>>(result.Items);
            return new PagedResult<ResponsePecaDto>(items, result.TotalCount, result.CurrentPage, result.PageSize);
        }

        /// <summary>
        /// Obtem uma peca pela designacao dentro de um molde.
        /// </summary>
        /// <param name="designacao">Designacao funcional da peca.</param>
        /// <param name="moldeId">Identificador do molde.</param>
        /// <returns>DTO da peca quando encontrada; nulo caso contrario.</returns>
        public async Task<ResponsePecaDto?> GetByDesignacaoAsync(string designacao, int moldeId)
        {
            var designacaoNormalizada = designacao.Trim();
            var peca = await _pecaRepository.GetByDesignacaoAsync(designacaoNormalizada, moldeId);
            return peca == null ? null : _mapper.Map<ResponsePecaDto>(peca);
        }

        /// <summary>
        /// Cria uma nova peca.
        /// </summary>
        /// <remarks>
        /// Fluxo critico:
        /// 1. Valida molde existente.
        /// 2. Valida designacao obrigatoria.
        /// 3. Garante unicidade da designacao dentro do molde.
        /// 4. Persiste a peca e devolve o DTO estavel da feature.
        /// </remarks>
        /// <param name="dto">Dados de criacao da peca.</param>
        /// <returns>DTO da peca criada.</returns>
        public async Task<ResponsePecaDto> CreateAsync(CreatePecaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Designacao))
                throw new ArgumentException("Designacao e obrigatoria.");

            var molde = await _moldeRepository.GetByIdAsync(dto.Molde_id);
            if (molde == null)
                throw new KeyNotFoundException($"Molde com ID {dto.Molde_id} nao encontrado.");

            var designacaoNormalizada = dto.Designacao.Trim();
            var existente = await _pecaRepository.GetByDesignacaoAsync(designacaoNormalizada, dto.Molde_id);
            if (existente != null)
                throw new ArgumentException("Ja existe uma peca com esta designacao neste molde.");

            var peca = _mapper.Map<Peca>(dto);
            peca.Designacao = designacaoNormalizada;

            await _pecaRepository.AddAsync(peca);

            _logger.LogInformation(
                "Peca {PecaId} criada com sucesso no molde {MoldeId}",
                peca.Peca_id,
                dto.Molde_id);

            return _mapper.Map<ResponsePecaDto>(peca);
        }

        /// <summary>
        /// Atualiza parcialmente uma peca existente.
        /// </summary>
        /// <remarks>
        /// Campos nao enviados no DTO devem manter o valor atual da entidade.
        /// </remarks>
        /// <param name="id">Identificador da peca a atualizar.</param>
        /// <param name="dto">Dados de atualizacao parcial.</param>
        /// <returns>Task de conclusao da atualizacao.</returns>
        public async Task UpdateAsync(int id, UpdatePecaDto dto)
        {
            var existente = await _pecaRepository.GetByIdAsync(id);
            if (existente == null)
                throw new KeyNotFoundException($"Peca com ID {id} nao encontrada.");

            if (!HasAnyChanges(dto))
                throw new ArgumentException("Pelo menos um campo deve ser informado para atualizacao.");

            if (!string.IsNullOrWhiteSpace(dto.Designacao))
            {
                var designacaoNormalizada = dto.Designacao.Trim();

                if (!string.Equals(existente.Designacao, designacaoNormalizada, StringComparison.OrdinalIgnoreCase))
                {
                    var duplicado = await _pecaRepository.GetByDesignacaoAsync(designacaoNormalizada, existente.Molde_id);
                    if (duplicado != null && duplicado.Peca_id != id)
                        throw new ArgumentException("Ja existe uma peca com esta designacao neste molde.");
                }
            }

            // Porque: update parcial deve preservar o estado atual quando o campo nao e enviado.
            _mapper.Map(dto, existente);

            await _pecaRepository.UpdateAsync(existente);

            _logger.LogInformation("Peca {PecaId} atualizada com sucesso", id);
        }

        /// <summary>
        /// Remove uma peca existente.
        /// </summary>
        /// <param name="id">Identificador da peca a remover.</param>
        /// <returns>Task de conclusao da remocao.</returns>
        public async Task DeleteAsync(int id)
        {
            var peca = await _pecaRepository.GetByIdAsync(id);
            if (peca == null)
                throw new KeyNotFoundException($"Peca com ID {id} nao encontrada.");

            await _pecaRepository.DeleteAsync(id);

            _logger.LogInformation("Peca {PecaId} removida com sucesso", id);
        }

        /// <summary>
        /// Verifica se o pedido de update contem pelo menos uma alteracao funcional.
        /// </summary>
        /// <param name="dto">DTO de atualizacao parcial.</param>
        /// <returns>True quando existe pelo menos um campo preenchido; false caso contrario.</returns>
        private static bool HasAnyChanges(UpdatePecaDto dto)
        {
            return !string.IsNullOrWhiteSpace(dto.Designacao)
                || dto.Prioridade.HasValue
                || dto.MaterialDesignacao != null
                || dto.MaterialRecebido.HasValue;
        }
    }
}
