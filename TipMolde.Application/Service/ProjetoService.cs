using AutoMapper;
using Microsoft.Extensions.Logging;
using TipMolde.Application.DTOs.ProjetoDTO;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Desenho.IProjeto;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Application.Service
{
    /// <summary>
    /// Implementa os casos de uso da feature Projeto.
    /// </summary>
    /// <remarks>
    /// Centraliza validacoes de negocio, atualizacao parcial e orquestracao
    /// entre DTOs, dominio e persistencia do agregado Projeto.
    /// </remarks>
    public class ProjetoService : IProjetoService
    {
        private readonly IProjetoRepository _projetoRepository;
        private readonly IMoldeRepository _moldeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProjetoService> _logger;

        /// <summary>
        /// Construtor de ProjetoService.
        /// </summary>
        /// <param name="projetoRepository">Repositorio do agregado Projeto.</param>
        /// <param name="moldeRepository">Repositorio usado para validar o molde referenciado.</param>
        /// <param name="mapper">Mapper para conversao entre DTOs e entidades.</param>
        /// <param name="logger">Logger para rastreabilidade das operacoes criticas.</param>
        public ProjetoService(
            IProjetoRepository projetoRepository,
            IMoldeRepository moldeRepository,
            IMapper mapper,
            ILogger<ProjetoService> logger)
        {
            _projetoRepository = projetoRepository;
            _moldeRepository = moldeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Lista projetos de forma paginada.
        /// </summary>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com DTOs de projeto.</returns>
        public async Task<PagedResult<ResponseProjetoDTO>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var result = await _projetoRepository.GetAllAsync(page, pageSize);
            var itens = _mapper.Map<IEnumerable<ResponseProjetoDTO>>(result.Items);
            return new PagedResult<ResponseProjetoDTO>(itens, result.TotalCount, result.CurrentPage, result.PageSize);
        }

        /// <summary>
        /// Obtem um projeto por identificador.
        /// </summary>
        /// <param name="id">Identificador interno do projeto.</param>
        /// <returns>DTO do projeto quando encontrado; nulo caso contrario.</returns>
        public async Task<ResponseProjetoDTO?> GetByIdAsync(int id)
        {
            var projeto = await _projetoRepository.GetByIdAsync(id);
            return projeto == null ? null : _mapper.Map<ResponseProjetoDTO>(projeto);
        }

        /// <summary>
        /// Obtem um projeto com as revisoes associadas.
        /// </summary>
        /// <param name="id">Identificador interno do projeto.</param>
        /// <returns>DTO enriquecido com revisoes quando encontrado; nulo caso contrario.</returns>
        public async Task<ResponseProjetoWithRevisoesDTO?> GetWithRevisoesAsync(int id)
        {
            var projeto = await _projetoRepository.GetWithRevisoesAsync(id);
            return projeto == null ? null : _mapper.Map<ResponseProjetoWithRevisoesDTO>(projeto);
        }

        /// <summary>
        /// Lista projetos associados a um molde.
        /// </summary>
        /// <param name="moldeId">Identificador do molde.</param>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com DTOs de projeto.</returns>
        public async Task<PagedResult<ResponseProjetoDTO>> GetByMoldeIdAsync(int moldeId, int page = 1, int pageSize = 10)
        {
            var projetos = await _projetoRepository.GetByMoldeIdAsync(moldeId, page, pageSize);
            var itens = _mapper.Map<IEnumerable<ResponseProjetoDTO>>(projetos.Items);
            return new PagedResult<ResponseProjetoDTO>(itens, projetos.TotalCount, projetos.CurrentPage, projetos.PageSize);
        }

        /// <summary>
        /// Cria um novo projeto.
        /// </summary>
        /// <remarks>
        /// Fluxo critico:
        /// 1. Valida campos obrigatorios.
        /// 2. Valida a existencia do molde referenciado.
        /// 3. Persiste o projeto com o caminho da pasta do servidor.
        /// </remarks>
        /// <param name="dto">Dados de criacao do projeto.</param>
        /// <returns>DTO do projeto criado.</returns>
        public async Task<ResponseProjetoDTO> CreateAsync(CreateProjetoDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NomeProjeto))
                throw new ArgumentException("Nome do projeto e obrigatorio.");

            if (string.IsNullOrWhiteSpace(dto.SoftwareUtilizado))
                throw new ArgumentException("Software utilizado e obrigatorio.");

            if (string.IsNullOrWhiteSpace(dto.CaminhoPastaServidor))
                throw new ArgumentException("Caminho da pasta no servidor e obrigatorio.");

            // Porque: validamos a existencia do Molde antes de persistir o Projeto para devolver
            // erro funcional controlado e evitar falha tecnica de chave estrangeira.
            var molde = await _moldeRepository.GetByIdAsync(dto.Molde_id);
            if (molde == null)
                throw new KeyNotFoundException($"Molde com ID {dto.Molde_id} nao encontrado.");

            var projeto = _mapper.Map<Projeto>(dto);

            await _projetoRepository.AddAsync(projeto);

            _logger.LogInformation(
                "Projeto {ProjetoId} criado com sucesso para o molde {MoldeId}",
                projeto.Projeto_id,
                dto.Molde_id);

            return _mapper.Map<ResponseProjetoDTO>(projeto);
        }

        /// <summary>
        /// Atualiza parcialmente um projeto existente.
        /// </summary>
        /// <remarks>
        /// Campos nao enviados no DTO sao preservados na entidade atual.
        /// </remarks>
        /// <param name="id">Identificador do projeto a atualizar.</param>
        /// <param name="dto">Dados de atualizacao parcial.</param>
        /// <returns>Task de conclusao da atualizacao.</returns>
        public async Task UpdateAsync(int id, UpdateProjetoDTO dto)
        {
            var existing = await _projetoRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Projeto com ID {id} nao encontrado.");

            if (!HasAnyChanges(dto))
                throw new ArgumentException("Pelo menos um campo deve ser informado para atualizacao.");

            _mapper.Map(dto, existing);

            await _projetoRepository.UpdateAsync(existing);

            _logger.LogInformation("Projeto {ProjetoId} atualizado com sucesso", id);
        }

        /// <summary>
        /// Remove um projeto existente.
        /// </summary>
        /// <param name="id">Identificador do projeto a remover.</param>
        /// <returns>Task de conclusao da remocao.</returns>
        public async Task DeleteAsync(int id)
        {
            var existing = await _projetoRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Projeto com ID {id} nao encontrado.");

            await _projetoRepository.DeleteAsync(id);

            _logger.LogInformation("Projeto {ProjetoId} removido com sucesso", id);
        }

        /// <summary>
        /// Verifica se o pedido de update contem pelo menos uma alteracao funcional.
        /// </summary>
        /// <param name="dto">DTO de atualizacao parcial.</param>
        /// <returns>True quando existe pelo menos um campo preenchido; false caso contrario.</returns>
        private static bool HasAnyChanges(UpdateProjetoDTO dto)
        {
            return !string.IsNullOrWhiteSpace(dto.NomeProjeto)
                || !string.IsNullOrWhiteSpace(dto.SoftwareUtilizado)
                || !string.IsNullOrWhiteSpace(dto.CaminhoPastaServidor)
                || dto.TipoProjeto.HasValue;
        }
    }
}
