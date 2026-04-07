using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.API.DTOs.FornecedorDTO;
using TipMolde.Core.Interface.IFornecedor;
using TipMolde.Core.Models;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FornecedorController : ControllerBase
    {
        private readonly IFornecedorService _service;

        public FornecedorController(IFornecedorService service)
        {
            _service = service;
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var fornecedores = await _service.GetAllAsync();
            return Ok(fornecedores.Select(ToResponse));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("by-id")]
        public async Task<IActionResult> GetById(int id)
        {
            var fornecedor = await _service.GetByIdAsync(id);
            if (fornecedor == null) return NotFound();
            return Ok(ToResponse(fornecedor));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            var fornecedores = await _service.SearchByNameAsync(searchTerm);
            return Ok(fornecedores.Select(ToResponse));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateFornecedorDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var fornecedor = new Fornecedor
            {
                Nome = dto.Nome.Trim(),
                NIF = dto.NIF.Trim(),
                Morada = dto.Morada,
                Email = dto.Email,
                Telefone = dto.Telefone
            };

            var created = await _service.CreateAsync(fornecedor);
            return CreatedAtAction(nameof(GetById), new { id = created.Fornecedor_id }, ToResponse(created));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFornecedorDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var fornecedor = new Fornecedor
            {
                Fornecedor_id = id,
                Nome = dto.Nome ?? string.Empty,
                NIF = dto.NIF ?? string.Empty,
                Morada = dto.Morada ?? string.Empty,
                Email = dto.Email,
                Telefone = dto.Telefone
            };

            await _service.UpdateAsync(fornecedor);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        private static ResponseFornecedorDTO ToResponse(Fornecedor f) => new()
        {
            FornecedorId = f.Fornecedor_id,
            Nome = f.Nome,
            NIF = f.NIF,
            Morada = f.Morada,
            Email = f.Email,
            Telefone = f.Telefone
        };
    }
}
