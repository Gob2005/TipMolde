using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.DTOs.ClienteDTO;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        [HttpGet("all-clientes")]
        public async Task<IActionResult> GetAllClientes()
        {
            var result = await _clienteService.GetAllAsync();
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
        [HttpGet("cliente-byID")]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            var response = _mapper.Map<ResponseClienteDTO>(cliente);
            if (cliente == null) return NotFound();
            return Ok(response);
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("cliente-with-encomendas")]
        public async Task<IActionResult> GetClienteWithEncomendas(int id)
        {
            var cliente = await _clienteService.GetClienteWithEncomendasAsync(id);
            var response = _mapper.Map<ResponseClienteWithEncomendasDTO>(cliente);  
            if (cliente == null) return NotFound();
            return Ok(response);
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL,GESTOR_DESENHO")]
        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            var clientes = await _clienteService.SearchByNameAsync(searchTerm);
            var response = _mapper.Map<IEnumerable<ResponseClienteDTO>>(clientes);
            return Ok(response);
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL,GESTOR_DESENHO")]
        [HttpGet("search-sigla")]
        public async Task<IActionResult> SearchBySigla([FromQuery] string searchTerm)
        {
            var clientes = await _clienteService.SearchBySiglaAsync(searchTerm);
            var response = _mapper.Map<IEnumerable<ResponseClienteDTO>>(clientes);
            return Ok(response);
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPost("create-cliente")]
        public async Task<IActionResult> CreateCliente([FromBody] CreateClienteDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var cliente = _mapper.Map<Cliente>(dto);
                var created = await _clienteService.CreateAsync(cliente);
                var response = _mapper.Map<ResponseClienteDTO>(created);

                _logger.LogInformation("Cliente {ClienteId} criado com sucesso", created.Cliente_id);

                return CreatedAtAction(
                    nameof(GetClienteById),
                    new { id = created.Cliente_id },
                    response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Erro ao criar cliente: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize (Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPut("update-cliente/{id:int}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] UpdateClienteDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var cliente = _mapper.Map<Cliente>(dto);
                cliente.Cliente_id = id;

                await _clienteService.UpdateAsync(cliente);

                _logger.LogInformation("Cliente {ClienteId} atualizado com sucesso", id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Cliente {ClienteId} não encontrado: {Message}", id, ex.Message);
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpDelete("delete-cliente")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            try
            {
                await _clienteService.DeleteAsync(id);
                _logger.LogInformation("Cliente {ClienteId} deletado com sucesso", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Cliente {ClienteId} não encontrado: {Message}", id, ex.Message);
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
