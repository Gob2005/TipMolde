using Microsoft.AspNetCore.Mvc;
using TipMolde.App.DTOs.ClienteDTO;
using TipMolde.Core.Interface.ICliente;
using TipMolde.Core.Models;

namespace TipMolde.App.Controllers
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

        [HttpGet("all-clientes")]
        public async Task<IActionResult> GetAllClientes()
        {
            var clientes = await _clienteService.GetAllClientesAsync();
            return Ok(clientes);
        }

        [HttpGet("cliente-byID")]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        [HttpPost("create-cliente")]
        public async Task<IActionResult> CreateCliente([FromBody] CreateClienteDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome)) return BadRequest("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.NIF)) return BadRequest("NIF e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.Sigla)) return BadRequest("Sigla e obrigatorio.");

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

            var createdCliente = await _clienteService.CreateClienteAsync(cliente);
            return CreatedAtAction(nameof(GetClienteById), new { id = createdCliente.Cliente_id }, createdCliente);
        }

        [HttpPut("update-cliente")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] UpdateClienteDTO dto)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null) return NotFound();

            cliente.Nome = !string.IsNullOrWhiteSpace(dto.Nome) ? dto.Nome.Trim() : cliente.Nome;
            cliente.Pais = !string.IsNullOrWhiteSpace(dto.Pais) ? dto.Pais.Trim() : cliente.Pais;
            cliente.Email = !string.IsNullOrWhiteSpace(dto.Email) ? dto.Email.Trim() : cliente.Email;
            cliente.Telefone = !string.IsNullOrWhiteSpace(dto.Telefone) ? dto.Telefone.Trim() : cliente.Telefone;
            cliente.NIF = !string.IsNullOrWhiteSpace(dto.NIF) ? dto.NIF.Trim() : cliente.NIF;
            cliente.Sigla = !string.IsNullOrWhiteSpace(dto.Sigla) ? dto.Sigla.Trim() : cliente.Sigla;

            await _clienteService.UpdateClienteAsync(cliente);
            return NoContent();
        }

        [HttpDelete("delete-cliente")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            await _clienteService.DeleteClienteAsync(id);
            return NoContent();
        }

        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            var clientes = await _clienteService.SearchByNameAsync(searchTerm);
            return Ok(clientes);
        }

        [HttpGet("search-sigla")]
        public async Task<IActionResult> SearchBySigla([FromQuery] string searchTerm)
        {
            var clientes = await _clienteService.SearchBySiglaAsync(searchTerm);
            return Ok(clientes);
        }
    }
}
