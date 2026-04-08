using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.API.DTOs.ProjetoDTO;
using TipMolde.Core.Interface.Desenho.IProjeto;
using TipMolde.Core.Models.Desenho;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjetoController : ControllerBase
    {
        private readonly IProjetoService _service;

        public ProjetoController(IProjetoService service)
        {
            _service = service;
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("by-id")]
        public async Task<IActionResult> GetById(int id)
        {
            var projeto = await _service.GetByIdAsync(id);
            if (projeto == null) return NotFound();
            return Ok(projeto);
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("with-revisoes")]
        public async Task<IActionResult> GetWithRevisoes(int id)
        {
            var projeto = await _service.GetWithRevisoesAsync(id);
            if (projeto == null) return NotFound();
            return Ok(projeto);
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("by-molde")]
        public async Task<IActionResult> GetByMoldeId(int moldeId) =>
            Ok(await _service.GetByMoldeIdAsync(moldeId));

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateProjetoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var projeto = new Projeto
            {
                NomeProjeto = dto.NomeProjeto.Trim(),
                SoftwareUtilizado = dto.SoftwareUtilizado.Trim(),
                TipoProjeto = dto.TipoProjeto,
                Molde_id = dto.Molde_id
            };

            var created = await _service.CreateAsync(projeto);
            return CreatedAtAction(nameof(GetById), new { id = created.Projeto_id }, created);
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjetoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var projeto = new Projeto
            {
                Projeto_id = id,
                NomeProjeto = dto.NomeProjeto ?? string.Empty,
                SoftwareUtilizado = dto.SoftwareUtilizado ?? string.Empty,
                TipoProjeto = dto.TipoProjeto ?? default
            };

            await _service.UpdateAsync(projeto);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
