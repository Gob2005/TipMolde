using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TipMolde.Application.Dtos.PedidoMaterialDto;
using TipMolde.Application.Interface.Comercio.IPedidoMaterial;

namespace TipMolde.API.Controllers
{
    /// <summary>
    /// Disponibiliza endpoints para gestao do ciclo de vida de pedidos de material.
    /// </summary>
    /// <remarks>
    /// O controlador limita-se a validar parametros HTTP, aplicar regras de autorizacao
    /// e delegar a logica funcional ao servico de aplicacao.
    /// </remarks>
    [ApiController]
    [Route("api/pedidos-material")]
    public class PedidoMaterialController : ControllerBase
    {
        private readonly IPedidoMaterialService _service;
        private readonly ILogger<PedidoMaterialController> _logger;

        /// <summary>
        /// Construtor de PedidoMaterialController.
        /// </summary>
        /// <param name="service">Servico responsavel pelos casos de uso de pedido de material.</param>
        /// <param name="logger">Logger para rastreabilidade das operacoes do controlador.</param>
        public PedidoMaterialController(
            IPedidoMaterialService service,
            ILogger<PedidoMaterialController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Lista pedidos de material com paginacao.
        /// </summary>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado HTTP com lista paginada de pedidos de material.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "Page e pageSize devem ser >= 1."));
            }

            var result = await _service.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Obtem um pedido de material pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do pedido.</param>
        /// <returns>Resultado HTTP com o pedido encontrado ou erro de nao encontrado.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pedido = await _service.GetByIdAsync(id);
            if (pedido == null)
            {
                return NotFound(CreateProblem(
                    StatusCodes.Status404NotFound,
                    "Recurso nao encontrado",
                    $"Pedido de material com ID {id} nao encontrado."));
            }

            return Ok(pedido);
        }

        /// <summary>
        /// Lista pedidos de material de um fornecedor.
        /// </summary>
        /// <param name="fornecedorId">Identificador do fornecedor.</param>
        /// <returns>Resultado HTTP com os pedidos associados ao fornecedor informado.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("fornecedores/{fornecedorId:int}")]
        public async Task<IActionResult> GetByFornecedorId(int fornecedorId)
        {
            var pedidos = await _service.GetByFornecedorIdAsync(fornecedorId);
            return Ok(pedidos);
        }

        /// <summary>
        /// Cria um novo pedido de material.
        /// </summary>
        /// <remarks>
        /// O servico valida fornecedor, pecas e duplicados antes de persistir o agregado completo.
        /// </remarks>
        /// <param name="dto">Dados de criacao do pedido e das respetivas linhas.</param>
        /// <returns>Resultado HTTP de criacao com o pedido persistido.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePedidoMaterialDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "Dados de criacao invalidos."));
            }

            var created = await _service.CreateAsync(dto);

            _logger.LogInformation("Pedido de material {PedidoId} criado com sucesso", created.PedidoMaterialId);

            return CreatedAtAction(nameof(GetById), new { id = created.PedidoMaterialId }, created);
        }

        /// <summary>
        /// Regista a rececao de um pedido de material.
        /// </summary>
        /// <remarks>
        /// Seguranca: o utilizador conferente e derivado do token autenticado para impedir impersonacao.
        /// A operacao atualiza o estado do pedido e desbloqueia as pecas associadas para producao.
        /// </remarks>
        /// <param name="id">Identificador do pedido a marcar como recebido.</param>
        /// <returns>Resultado HTTP sem conteudo quando a rececao e concluida.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL,GESTOR_PRODUCAO")]
        [HttpPut("{id:int}/rececao")]
        public async Task<IActionResult> RegistarRececao(int id)
        {
            var userId = GetAuthenticatedUserId();

            await _service.RegistarRececaoAsync(id, userId);

            _logger.LogInformation(
                "Rececao do pedido de material {PedidoId} registada pelo utilizador autenticado {UserId}",
                id,
                userId);

            return NoContent();
        }

        /// <summary>
        /// Remove um pedido de material pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do pedido a remover.</param>
        /// <returns>Resultado HTTP sem conteudo quando a remocao e concluida.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            _logger.LogInformation("Pedido de material {PedidoId} removido com sucesso", id);

            return NoContent();
        }

        /// <summary>
        /// Extrai o identificador do utilizador autenticado a partir dos claims do JWT.
        /// </summary>
        /// <returns>Identificador numerico do utilizador autenticado.</returns>
        private int GetAuthenticatedUserId()
        {
            var userIdClaim =
                User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("id");

            if (!int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Utilizador autenticado invalido no token.");

            return userId;
        }

        /// <summary>
        /// Cria um objeto de erro padrao no formato ProblemDetails.
        /// </summary>
        /// <param name="status">Codigo de estado HTTP da resposta.</param>
        /// <param name="title">Titulo curto do problema.</param>
        /// <param name="detail">Descricao detalhada do problema.</param>
        /// <returns>Instancia de ProblemDetails preenchida com o contexto do pedido.</returns>
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