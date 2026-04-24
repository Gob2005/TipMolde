using AutoMapper;
using Microsoft.Extensions.Logging;
using TipMolde.Application.Dtos.FasesProducaoDto;
using TipMolde.Application.Exceptions;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Service
{
    /// <summary>
    /// Implementa os casos de uso da feature FasesProducao.
    /// </summary>
    /// <remarks>
    /// Centraliza validacoes de negocio, conversao entre DTOs e entidade
    /// e protecao contra remocao de fases em uso por maquinas.
    /// </remarks>
    public class FasesProducaoService : IFasesProducaoService
    {
        private readonly IFasesProducaoRepository _fpRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FasesProducaoService> _logger;

        /// <summary>
        /// Construtor de FasesProducaoService.
        /// </summary>
        /// <param name="fpRepository">Repositorio da feature FasesProducao.</param>
        /// <param name="mapper">Mapper para conversao entre DTOs e entidade.</param>
        /// <param name="logger">Logger para rastreabilidade das operacoes criticas.</param>
        public FasesProducaoService(
            IFasesProducaoRepository fpRepository,
            IMapper mapper,
            ILogger<FasesProducaoService> logger)
        {
            _fpRepository = fpRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Lista fases de producao com paginacao.
        /// </summary>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com DTOs de resposta.</returns>
        public async Task<PagedResult<ResponseFasesProducaoDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var result = await _fpRepository.GetAllAsync(page, pageSize);
            var items = _mapper.Map<IEnumerable<ResponseFasesProducaoDto>>(result.Items);

            return new PagedResult<ResponseFasesProducaoDto>(
                items,
                result.TotalCount,
                result.CurrentPage,
                result.PageSize);
        }

        /// <summary>
        /// Obtem uma fase por identificador.
        /// </summary>
        /// <param name="id">Identificador interno da fase.</param>
        /// <returns>DTO da fase quando encontrada; nulo caso contrario.</returns>
        public async Task<ResponseFasesProducaoDto?> GetByIdAsync(int id)
        {
            var fase = await _fpRepository.GetByIdAsync(id);
            return fase == null ? null : _mapper.Map<ResponseFasesProducaoDto>(fase);
        }

        /// <summary>
        /// Cria uma nova fase de producao.
        /// </summary>
        /// <remarks>
        /// Fluxo critico:
        /// 1. Exige nome explicito no contrato.
        /// 2. Valida unicidade funcional antes da escrita.
        /// 3. Mantem a traducao do conflito tecnico no repository como ultima garantia.
        /// </remarks>
        /// <param name="dto">Dados de criacao da fase.</param>
        /// <returns>DTO da fase criada.</returns>
        public async Task<ResponseFasesProducaoDto> CreateAsync(CreateFasesProducaoDto dto)
        {
            var existing = await _fpRepository.GetByNomeAsync(dto.Nome);
            if (existing != null)
                throw new BusinessConflictException("Ja existe uma fase de producao com esse nome.");

            var fase = _mapper.Map<FasesProducao>(dto);
            fase.Nome = dto.Nome;

            var created = await _fpRepository.CreateAsync(fase);

            _logger.LogInformation(
                "Fase de producao {FaseId} criada com sucesso com o nome {Nome}.",
                created.Fases_producao_id,
                created.Nome);

            return _mapper.Map<ResponseFasesProducaoDto>(created);
        }

        /// <summary>
        /// Atualiza parcialmente uma fase existente.
        /// </summary>
        /// <remarks>
        /// Campos nao enviados mantem o valor atual.
        /// </remarks>
        /// <param name="id">Identificador da fase a atualizar.</param>
        /// <param name="dto">Dados de atualizacao parcial.</param>
        /// <returns>Task de conclusao da atualizacao.</returns>
        public async Task UpdateAsync(int id, UpdateFasesProducaoDto dto)
        {
            var existing = await _fpRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Fase de producao com ID {id} nao encontrada.");

            if (!HasAnyChanges(dto))
                throw new ArgumentException("Pelo menos um campo deve ser informado para atualizacao.");

            if (dto.Nome.HasValue)
            {
                var novoNome = dto.Nome.Value;
                if (existing.Nome != novoNome)
                {
                    var byNome = await _fpRepository.GetByNomeAsync(novoNome);
                    if (byNome != null && byNome.Fases_producao_id != id)
                        throw new BusinessConflictException("Ja existe uma fase de producao com esse nome.");
                }
            }

            _mapper.Map(dto, existing);

            await _fpRepository.UpdateExistingAsync(existing);

            _logger.LogInformation("Fase de producao {FaseId} atualizada com sucesso.", id);
        }

        /// <summary>
        /// Remove uma fase de producao.
        /// </summary>
        /// <remarks>
        /// Regra de negocio: fases ainda usadas por maquinas nao podem ser removidas.
        /// </remarks>
        /// <param name="id">Identificador da fase a remover.</param>
        /// <returns>Task de conclusao da remocao.</returns>
        public async Task DeleteAsync(int id)
        {
            var existing = await _fpRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Fase de producao com ID {id} nao encontrada.");

            if (await _fpRepository.HasMaquinasAssociadasAsync(id))
                throw new BusinessConflictException(
                    "Nao e possivel eliminar a fase de producao porque existem maquinas associadas.");

            await _fpRepository.DeleteAsync(id);

            _logger.LogInformation("Fase de producao {FaseId} removida com sucesso.", id);
        }

        /// <summary>
        /// Verifica se o DTO de update contem pelo menos uma alteracao funcional.
        /// </summary>
        /// <param name="dto">DTO de atualizacao parcial.</param>
        /// <returns>True quando existe pelo menos um campo preenchido; false caso contrario.</returns>
        private static bool HasAnyChanges(UpdateFasesProducaoDto dto)
        {
            return dto.Nome != null || !string.IsNullOrWhiteSpace(dto.Descricao);
        }
    }
}
