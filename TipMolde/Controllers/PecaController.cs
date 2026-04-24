using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.DTOs.PecaDTO;
using TipMolde.Application.Interface.Producao.IPeca;

namespace TipMolde.API.Controllers
{
    /// <summary>
    /// Disponibiliza endpoints HTTP para a feature Peca.
    /// </summary>
    /// <remarks>
    /// O controller valida input HTTP e delega regras de negocio ao servico.
    /// </remarks>
    [ApiController]
    [Route("api/pecas")]
    public class PecaController : ControllerBase
    {
        private readonly IPecaService _pecaService;
        private readonly ILogger<PecaController> _logger;

        /// <summary>
        /// Construtor de PecaController.
        /// </summary>
        /// <param name="pecaService">Servico responsavel pelos casos de uso da feature Peca.</param>
        /// <param name="logger">Logger para rastreabilidade das operacoes HTTP.</param>
        public PecaController(IPecaService pecaService, ILogger<PecaController> logger)
        {
            _pecaService = pecaService;
            _logger = logger;
        }

        /// <summary>
        /// Lista pecas com paginacao.
        /// </summary>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>HTTP 200 com resultado paginado; HTTP 400 para paginacao invalida.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(CreateProblem(StatusCodes.Status400BadRequest, "Pedido invalido", "Page e pageSize devem ser >= 1."));

            var result = await _pecaService.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Obtem uma peca por ID.
        /// </summary>
        /// <param name="id">Identificador interno da peca.</param>
        /// <returns>HTTP 200 com a peca; HTTP 404 quando nao encontrada.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var peca = await _pecaService.GetByIdAsync(id);
            if (peca == null)
                return NotFound(CreateProblem(StatusCodes.Status404NotFound, "Recurso nao encontrado", $"Peca com ID {id} nao encontrada."));

            return Ok(peca);
        }

        /// <summary>
        /// Lista pecas de um molde com paginacao.
        /// </summary>
        /// <param name="moldeId">Identificador do molde.</param>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>HTTP 200 com resultado paginado; HTTP 400 para paginacao invalida.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("por-molde/{moldeId:int}")]
        public async Task<IActionResult> GetByMoldeId(int moldeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(CreateProblem(StatusCodes.Status400BadRequest, "Pedido invalido", "Page e pageSize devem ser >= 1."));

            var result = await _pecaService.GetByMoldeIdAsync(moldeId, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Obtem uma peca pela designacao dentro de um molde.
        /// </summary>
        /// <param name="designacao">Designacao funcional da peca.</param>
        /// <param name="moldeId">Identificador do molde.</param>
        /// <returns>HTTP 200 com a peca; HTTP 400 quando a query e invalida; HTTP 404 quando nao encontrada.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("por-designacao")]
        public async Task<IActionResult> GetByDesignacao([FromQuery] string designacao, [FromQuery] int moldeId)
        {
            if (string.IsNullOrWhiteSpace(designacao))
                return BadRequest(CreateProblem(StatusCodes.Status400BadRequest, "Pedido invalido", "Designacao e obrigatoria."));

            var peca = await _pecaService.GetByDesignacaoAsync(designacao, moldeId);
            if (peca == null)
                return NotFound(CreateProblem(StatusCodes.Status404NotFound, "Recurso nao encontrado", $"Peca '{designacao.Trim()}' nao encontrada no molde {moldeId}."));

            return Ok(peca);
        }

        /// <summary>
        /// Cria uma nova peca.
        /// </summary>
        /// <param name="dto">Dados de criacao da peca.</param>
        /// <returns>HTTP 201 com a peca criada; HTTP 400 quando o body e invalido.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePecaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(CreateProblem(StatusCodes.Status400BadRequest, "Pedido invalido", "Dados de criacao invalidos."));

            var created = await _pecaService.CreateAsync(dto);

            _logger.LogInformation("Controller: Peca {PecaId} criada", created.PecaId);

            return CreatedAtAction(nameof(GetById), new { id = created.PecaId }, created);
        }

        /// <summary>
        /// Atualiza parcialmente uma peca.
        /// </summary>
        /// <remarks>
        /// Campos nao enviados devem manter o valor atual da peca.
        /// </remarks>
        /// <param name="id">Identificador da peca a atualizar.</param>
        /// <param name="dto">Dados de atualizacao parcial.</param>
        /// <returns>HTTP 204 quando a atualizacao e concluida; HTTP 400 quando o body e invalido.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePecaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(CreateProblem(StatusCodes.Status400BadRequest, "Pedido invalido", "Dados de atualizacao invalidos."));

            await _pecaService.UpdateAsync(id, dto);

            _logger.LogInformation("Controller: Peca {PecaId} atualizada", id);

            return NoContent();
        }

        /// <summary>
        /// Remove uma peca.
        /// </summary>
        /// <param name="id">Identificador da peca a remover.</param>
        /// <returns>HTTP 204 quando a remocao e concluida.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _pecaService.DeleteAsync(id);

            _logger.LogInformation("Controller: Peca {PecaId} removida", id);

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
