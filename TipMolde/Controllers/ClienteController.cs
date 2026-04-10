using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using TipMolde.Application.DTOs.ClienteDTO;
using TipMolde.Application.DTOs.EncomendaDTO;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL,GESTOR_DESENHO")]
        [HttpGet("all-clientes")]
        public async Task<IActionResult> GetAllClientes()
        {
            var result = await _clienteService.GetAllAsync();
            return Ok(new
            {
                result.TotalCount,
                result.CurrentPage,
                result.PageSize,
                Items = result.Items.Select(ToResponse)
            });
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL,GESTOR_DESENHO")]
        [HttpGet("cliente-byID")]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            if (cliente == null) return NotFound();
            return Ok(ToResponse(cliente));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("cliente-with-encomendas")]
        public async Task<IActionResult> GetClienteWithEncomendas(int id)
        {
            var cliente = await _clienteService.GetClienteWithEncomendasAsync(id);
            if (cliente == null) return NotFound();
            return Ok(ToResponseWithEncomendas(cliente));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL,GESTOR_DESENHO")]
        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            var clientes = await _clienteService.SearchByNameAsync(searchTerm);
            return Ok(clientes.Select(ToResponse));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL,GESTOR_DESENHO")]
        [HttpGet("search-sigla")]
        public async Task<IActionResult> SearchBySigla([FromQuery] string searchTerm)
        {
            var clientes = await _clienteService.SearchBySiglaAsync(searchTerm);
            return Ok(clientes.Select(ToResponse));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPost("create-cliente")]
        public async Task<IActionResult> CreateCliente([FromBody] CreateClienteDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cliente = new Cliente
            {
                Nome = dto.Nome.Trim(),
                Pais = dto.Pais,
                Email = dto.Email,
                Telefone = dto.Telefone,
                NIF = dto.NIF.Trim(),
                Sigla = dto.Sigla.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            var createdCliente = await _clienteService.CreateAsync(cliente);
            return CreatedAtAction(nameof(GetClienteById), new { id = createdCliente.Cliente_id }, createdCliente);
        }

        [Authorize (Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPut("update-cliente/{id:int}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] UpdateClienteDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cliente = new Cliente
            {
                Cliente_id = id,
                Nome = dto.Nome ?? string.Empty,
                NIF = dto.NIF ?? string.Empty,
                Sigla = dto.Sigla ?? string.Empty,
                Pais = dto.Pais,
                Email = dto.Email,
                Telefone = dto.Telefone
            };

            await _clienteService.UpdateAsync(cliente);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpDelete("delete-cliente")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            await _clienteService.DeleteAsync(id);
            return NoContent();
        }

        private static ResponseClienteWithEncomendasDTO ToResponseWithEncomendas(Cliente c) => new()
        {
            ClienteId = c.Cliente_id,
            Nome = c.Nome,
            Sigla = c.Sigla,
            Pais = c.Pais,
            Email = c.Email,
            Telefone = c.Telefone,
            NIF = c.NIF,
            Encomendas = c.Encomendas.Select(e => new ResponseEncomendaDTO
            {
                Encomenda_id = e.Encomenda_id,
                NumeroEncomendaCliente = e.NumeroEncomendaCliente,
                Estado = e.Estado,
                Cliente_id = e.Cliente_id,
                NomeCliente = c.Nome,
            })
        };

        private static ResponseClienteDTO ToResponse(Cliente c) => new()
        {
            ClienteId = c.Cliente_id,
            Nome = c.Nome,
            Sigla = c.Sigla,
            Pais = c.Pais,
            Email = c.Email,
            Telefone = c.Telefone,
            NIF = c.NIF
        };
    }
}
