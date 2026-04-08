using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.API.DTOs.MaquinaDTO;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.Producao.IMaquina;
using TipMolde.Core.Models.Producao;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaquinaController : ControllerBase
    {
        private readonly IMaquinaService _service;

        public MaquinaController(IMaquinaService service)
        {
            _service = service;
        }

        [Authorize(Roles = "ADMIN,GESTOR_PRODUCAO")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var maquinas = await _service.GetAllAsync();
            return Ok(maquinas.Select(ToResponse));
        }

        [Authorize(Roles = "ADMIN,GESTOR_PRODUCAO")]
        [HttpGet("by-id")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var maquina = await _service.GetByIdAsync(id);
            if (maquina == null) return NotFound();
            return Ok(ToResponse(maquina));
        }

        [Authorize(Roles = "ADMIN,GESTOR_PRODUCAO")]
        [HttpGet("by-estado")]
        public async Task<IActionResult> GetByEstado([FromQuery] EstadoMaquina estado)
        {
            var maquinas = await _service.GetByEstadoAsync(estado);
            return Ok(maquinas.Select(ToResponse));
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateMaquinaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var maquina = new Maquina
            {
                Maquina_id = dto.Maquina_id,
                NomeModelo = dto.NomeModelo.Trim(),
                IpAddress = dto.IpAddress,
                Estado = dto.Estado
            };

            var created = await _service.CreateAsync(maquina);
            return CreatedAtAction(nameof(GetById), new { id = created.Maquina_id }, ToResponse(created));
        }

        [Authorize(Roles = "ADMIN,GESTOR_PRODUCAO")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMaquinaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var maquina = new Maquina
            {
                Maquina_id = id,
                NomeModelo = dto.NomeModelo ?? string.Empty,
                IpAddress = dto.IpAddress,
                Estado = dto.Estado
            };

            await _service.UpdateAsync(maquina);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        private static ResponseMaquinaDTO ToResponse(Maquina m) => new()
        {
            Maquina_id = m.Maquina_id,
            NomeModelo = m.NomeModelo,
            IpAddress = m.IpAddress,
            Estado = m.Estado
        };
    }
}
