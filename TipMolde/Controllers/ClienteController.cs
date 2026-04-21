using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.DTOs.ClienteDTO;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.API.Controllers
{
    /// <summary>
    /// Disponibiliza endpoints para gestao de clientes no modulo comercial.
    /// </summary>
    /// <remarks>
    /// Recebe pedidos HTTP, valida parametros de entrada e delega regras de negocio ao servico de cliente.
    /// </remarks>
    [ApiController]
    [Route("api/clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly IMapper _mapper;
        private readonly ILogger<ClienteController> _logger;

        /// <summary>
        /// Construtor de ClienteController.
        /// </summary>
        /// <param name="clienteService">Servico responsavel pelos casos de uso de cliente.</param>
        /// <param name="mapper">Mapper para conversao entre entidades de dominio e DTOs.</param>
        /// <param name="logger">Logger para registo de operacoes do controlador.</param>
        public ClienteController(
            IClienteService clienteService,
            IMapper mapper,
            ILogger<ClienteController> logger)
        {
            _clienteService = clienteService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Lista clientes com paginacao.
        /// </summary>
        /// <remarks>
        /// Valida os parametros de pagina e devolve metadados de paginacao com os itens convertidos para DTO.
        /// </remarks>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado HTTP com lista paginada de clientes.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL,GESTOR_DESENHO")]
        [HttpGet]
        public async Task<IActionResult> GetAllClientes([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "Page e pageSize devem ser >= 1."));

            var result = await _clienteService.GetAllAsync(page, pageSize);
            var response = _mapper.Map<IEnumerable<ResponseClienteDTO>>(result.Items);

            return Ok(new
            {
                result.TotalCount,
                result.CurrentPage,
                result.PageSize,
                Items = response
            });
        }

        /// <summary>
        /// Obtem um cliente pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do cliente.</param>
        /// <returns>Resultado HTTP com o cliente encontrado ou erro de nao encontrado.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL,GESTOR_DESENHO")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null)
            {
                return NotFound(CreateProblem(
                    StatusCodes.Status404NotFound,
                    "Recurso nao encontrado",
                    $"Cliente com ID {id} nao encontrado."));
            }

            var response = _mapper.Map<ResponseClienteDTO>(cliente);
            return Ok(response);
        }

        /// <summary>
        /// Obtem um cliente incluindo a respetiva colecao de encomendas.
        /// </summary>
        /// <param name="id">Identificador unico do cliente.</param>
        /// <returns>Resultado HTTP com dados do cliente e encomendas associadas.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("{id:int}/encomendas")]
        public async Task<IActionResult> GetClienteWithEncomendas(int id)
        {
            var cliente = await _clienteService.GetClienteWithEncomendasAsync(id);
            if (cliente == null)
            {
                return NotFound(CreateProblem(
                    StatusCodes.Status404NotFound,
                    "Recurso nao encontrado",
                    $"Cliente com ID {id} nao encontrado."));
            }

            var response = _mapper.Map<ResponseClienteWithEncomendasDTO>(cliente);
            return Ok(response);
        }

        /// <summary>
        /// Pesquisa clientes por nome.
        /// </summary>
        /// <param name="searchTerm">Termo parcial de pesquisa aplicado ao nome do cliente.</param>
        /// <returns>Resultado HTTP com clientes que correspondem ao termo informado.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL,GESTOR_DESENHO")]
        [HttpGet("search/by-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "O parametro searchTerm e obrigatorio."));
            }

            var clientes = await _clienteService.SearchByNameAsync(searchTerm);
            var response = _mapper.Map<IEnumerable<ResponseClienteDTO>>(clientes);
            return Ok(response);
        }

        /// <summary>
        /// Pesquisa clientes por sigla.
        /// </summary>
        /// <param name="searchTerm">Termo parcial de pesquisa aplicado a sigla do cliente.</param>
        /// <returns>Resultado HTTP com clientes que correspondem ao termo informado.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL,GESTOR_DESENHO")]
        [HttpGet("search/by-sigla")]
        public async Task<IActionResult> SearchBySigla([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "O parametro searchTerm e obrigatorio."));
            }

            var clientes = await _clienteService.SearchBySiglaAsync(searchTerm);
            var response = _mapper.Map<IEnumerable<ResponseClienteDTO>>(clientes);
            return Ok(response);
        }

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <remarks>
        /// Converte o DTO de entrada para entidade de dominio e devolve o recurso criado com localizacao.
        /// </remarks>
        /// <param name="dto">Dados de criacao do cliente.</param>
        /// <returns>Resultado HTTP de criacao com o cliente persistido.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPost]
        public async Task<IActionResult> CreateCliente([FromBody] CreateClienteDTO dto)
        {
            var cliente = _mapper.Map<Cliente>(dto);
            var created = await _clienteService.CreateAsync(cliente);
            var response = _mapper.Map<ResponseClienteDTO>(created);

            _logger.LogInformation("Cliente {ClienteId} criado com sucesso", created.Cliente_id);

            return CreatedAtAction(nameof(GetClienteById), new { id = created.Cliente_id }, response);
        }

        /// <summary>
        /// Atualiza os dados de um cliente existente.
        /// </summary>
        /// <param name="id">Identificador do cliente a atualizar.</param>
        /// <param name="dto">Dados enviados para atualizacao do cliente.</param>
        /// <returns>Resultado HTTP sem conteudo quando a atualizacao e concluida.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] UpdateClienteDTO dto)
        {
            var cliente = _mapper.Map<Cliente>(dto);
            cliente.Cliente_id = id;

            await _clienteService.UpdateAsync(cliente);

            _logger.LogInformation("Cliente {ClienteId} atualizado com sucesso", id);

            return NoContent();
        }

        /// <summary>
        /// Remove um cliente pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do cliente a remover.</param>
        /// <returns>Resultado HTTP sem conteudo quando a remocao e concluida.</returns>
        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            await _clienteService.DeleteAsync(id);

            _logger.LogInformation("Cliente {ClienteId} removido com sucesso", id);

            return NoContent();
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
