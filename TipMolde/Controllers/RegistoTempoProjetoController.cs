using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.DTOs.RegistoTempoProjetoDTO;
using TipMolde.Application.Interface.Desenho.IRegistoTempoProjeto;
using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistoTempoProjetoController : ControllerBase
    {
        private readonly IRegistoTempoProjetoService _service;

        public RegistoTempoProjetoController(IRegistoTempoProjetoService service)
        {
            _service = service;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("historico-projeto")]
        public async Task<IActionResult> GetHistorico(
            [FromQuery] int projetoId,
            [FromQuery] int autorId)
        {
            var historico = await _service.GetHistoricoAsync(projetoId, autorId);
            return Ok(historico);
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateRegistoTempoProjetoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var registo = new RegistoTempoProjeto
            {
                Estado_tempo = dto.Estado_tempo,
                Projeto_id = dto.Projeto_id,
                Autor_id = dto.Autor_id,
            };

            var created = await _service.CreateRegistoAsync(registo);
            return Ok(created);
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
