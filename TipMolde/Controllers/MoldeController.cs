using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.Dtos.MoldeDto;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Application.Interface.Relatorios;

namespace TipMolde.API.Controllers
{
    /// <summary>
    /// Disponibiliza endpoints HTTP para a feature Molde.
    /// </summary>
    /// <remarks>
    /// O controller valida input HTTP e delega regras de negocio ao servico.
    /// </remarks>
    [ApiController]
    [Route("api/moldes")]
    public class MoldeController : ControllerBase
    {
        private readonly IMoldeService _moldeService;
        private readonly IRelatorioService _relatorioService;
        private readonly ILogger<MoldeController> _logger;

        /// <summary>
        /// Construtor de MoldeController.
        /// </summary>
        /// <param name="moldeService">Servico responsavel pelos casos de uso da feature Molde.</param>
        /// <param name="relatorioService">Servico responsavel pela geracao de relatorios do molde.</param>
        /// <param name="logger">Logger para rastreabilidade das operacoes HTTP.</param>
        public MoldeController(
            IMoldeService moldeService,
            IRelatorioService relatorioService,
            ILogger<MoldeController> logger)
        {
            _moldeService = moldeService;
            _relatorioService = relatorioService;
            _logger = logger;
        }

        /// <summary>
        /// Lista moldes com paginacao.
        /// </summary>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>HTTP 200 com resultado paginado; HTTP 400 para paginacao invalida.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(CreateProblem(StatusCodes.Status400BadRequest, "Pedido invalido", "Page e pageSize devem ser >= 1."));

            var result = await _moldeService.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Obtem um molde por ID.
        /// </summary>
        /// <param name="id">Identificador interno do molde.</param>
        /// <returns>HTTP 200 com o molde; HTTP 404 quando nao encontrado.</returns>
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var molde = await _moldeService.GetByIdAsync(id);
            if (molde == null)
                return NotFound(CreateProblem(StatusCodes.Status404NotFound, "Recurso nao encontrado", $"Molde com ID {id} nao encontrado."));

            return Ok(molde);
        }

        /// <summary>
        /// Lista moldes associados a uma encomenda.
        /// </summary>
        /// <param name="encomendaId">Identificador da encomenda.</param>
        /// <returns>HTTP 200 com a colecao de moldes associados.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("por-encomenda/{encomendaId:int}")]
        public async Task<IActionResult> GetByEncomendaId(int encomendaId)
        {
            var moldes = await _moldeService.GetByEncomendaIdAsync(encomendaId);
            return Ok(moldes);
        }

        /// <summary>
        /// Obtem um molde pelo numero funcional.
        /// </summary>
        /// <param name="numero">Numero funcional do molde.</param>
        /// <returns>HTTP 200 com o molde; HTTP 400 quando o numero e invalido; HTTP 404 quando nao encontrado.</returns>
        [Authorize]
        [HttpGet("por-numero")]
        public async Task<IActionResult> GetByNumero([FromQuery] string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return BadRequest(CreateProblem(StatusCodes.Status400BadRequest, "Pedido invalido", "Numero do molde e obrigatorio."));

            var molde = await _moldeService.GetByNumeroAsync(numero);
            if (molde == null)
                return NotFound(CreateProblem(StatusCodes.Status404NotFound, "Recurso nao encontrado", $"Molde com numero '{numero.Trim()}' nao encontrado."));

            return Ok(molde);
        }

        /// <summary>
        /// Exporta o ciclo de vida do molde para PDF.
        /// </summary>
        /// <param name="id">Identificador interno do molde.</param>
        /// <returns>Ficheiro PDF gerado para o molde indicado.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/ciclo-vida-pdf")]
        public async Task<IActionResult> ExportCicloVidaPdf(int id)
        {
            var result = await _relatorioService.GerarCicloVidaMoldePdfAsync(id);

            _logger.LogInformation("PDF de ciclo de vida exportado para o molde {MoldeId}", id);

            return File(result.Content, "application/pdf", result.FileName);
        }

        /// <summary>
        /// Cria um novo molde.
        /// </summary>
        /// <remarks>
        /// O contrato cria o agregado Molde com especificacoes tecnicas e associacao inicial a uma encomenda.
        /// </remarks>
        /// <param name="dto">Dados de criacao do molde.</param>
        /// <returns>HTTP 201 com o molde criado; HTTP 400 quando o body e invalido.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMoldeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(CreateProblem(StatusCodes.Status400BadRequest, "Pedido invalido", "Dados de criacao invalidos."));

            var created = await _moldeService.CreateAsync(dto);

            _logger.LogInformation("Controller: Molde {MoldeId} criado", created.MoldeId);

            return CreatedAtAction(nameof(GetById), new { id = created.MoldeId }, created);
        }

        /// <summary>
        /// Atualiza parcialmente um molde.
        /// </summary>
        /// <remarks>
        /// Campos nao enviados sao preservados no registo atual.
        /// </remarks>
        /// <param name="id">Identificador do molde a atualizar.</param>
        /// <param name="dto">Dados de atualizacao parcial.</param>
        /// <returns>HTTP 204 quando a atualizacao e concluida; HTTP 400 quando o body e invalido.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMoldeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(CreateProblem(StatusCodes.Status400BadRequest, "Pedido invalido", "Dados de criacao invalidos."));

            await _moldeService.UpdateAsync(id, dto);

            _logger.LogInformation("Controller: Molde {MoldeId} atualizado", id);

            return NoContent();
        }

        /// <summary>
        /// Remove um molde.
        /// </summary>
        /// <param name="id">Identificador do molde a remover.</param>
        /// <returns>HTTP 204 quando a remocao e concluida.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _moldeService.DeleteAsync(id);

            _logger.LogInformation("Controller: Molde {MoldeId} removido", id);

            return NoContent();
        }

        /// <summary>
        /// Cria objeto ProblemDetails para respostas de erro no controller.
        /// </summary>
        /// <param name="status">Codigo HTTP do erro.</param>
        /// <param name="title">Titulo curto do erro.</param>
        /// <param name="detail">Detalhe funcional do erro.</param>
        /// <returns>Objeto ProblemDetails preenchido com contexto do request atual.</returns>
        private ProblemDetails CreateProblem(int status, string title, string detail)
        {
            return new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = HttpContext?.Request?.Path
            };
        }
    }
}
