using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.DTOs.ClienteDTO;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly IMapper _mapper;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(
            IClienteService clienteService,
            IMapper mapper,
            ILogger<ClienteController> logger)
        {
            _clienteService = clienteService;
            _mapper = mapper;
            _logger = logger;
        }

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

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            await _clienteService.DeleteAsync(id);

            _logger.LogInformation("Cliente {ClienteId} removido com sucesso", id);

            return NoContent();
        }

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
