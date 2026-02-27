using Microsoft.AspNetCore.Mvc;
using TipMolde.App.DTOs.ClienteDTO;
using TipMolde.App.DTOs.UserDTO;
using TipMolde.Core.Interface.ICliente;
using TipMolde.Core.Models;

namespace TipMolde.App.Controllers
{
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

         [HttpGet]
         public async Task<IActionResult> GetAllClientes()
            {
                var clientes = await _clienteService.GetAllClientesAsync();
                return Ok(clientes);
        }

        [HttpGet]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCliente(CreateClienteDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome)) return BadRequest("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.NIF)) return BadRequest("NIF e obrigatorio.");

            var cliente = new Cliente
            {
                Nome = dto.Nome,
                Pais = dto.Pais,
                Email = dto.Email,
                Telefone = dto.Telefone,
                NIF = dto.NIF,
                CreatedAt = DateTime.UtcNow
            };
            var createdCliente = await _clienteService.CreateClienteAsync(cliente);
            return CreatedAtAction(nameof(GetClienteById), new { id = createdCliente.Cliente_id }, createdCliente);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCliente(int id, UpdateClienteDTO dto)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null) return NotFound();

            cliente.Nome = dto.Nome ?? cliente.Nome;
            cliente.Pais = dto.Pais ?? cliente.Pais;
            cliente.Email = dto.Email ?? cliente.Email;
            cliente.Telefone = dto.Telefone ?? cliente.Telefone;
            cliente.NIF = dto.NIF ?? cliente.NIF;

            await _clienteService.UpdateClienteAsync(cliente);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            await _clienteService.DeleteClienteAsync(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> SearchByName(string searchTerm)
        {
            var clientes = await _clienteService.SearchByNameAsync(searchTerm);
            return Ok(clientes);
        }
    }
}
