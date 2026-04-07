using Microsoft.AspNetCore.Mvc;
using TipMolde.API.DTOs.RegistoProducaoDTO;
using TipMolde.Core.Interface.IRegistosProducao;
using TipMolde.Core.Models;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistosProducaoController : ControllerBase
    {
        private readonly IRegistosProducaoService _registosProducaoService;

        public RegistosProducaoController(IRegistosProducaoService registosProducaoService)
        {
            _registosProducaoService = registosProducaoService;
        }

        [HttpGet("all-registos_producao")]
        public async Task<IActionResult> GetAllRegistos_producao()
        {
            var registosProducao = await _registosProducaoService.GetAllRegistosProducaoAsync();
            return Ok(registosProducao);
        }

        [HttpGet("registos_producao-byID")]
        public async Task<IActionResult> GetRegistos_producaoById(int id)
        {
            var registoProducao = await _registosProducaoService.GetRegistoProducaoByIdAsync(id);
            if (registoProducao == null) return NotFound();
            return Ok(registoProducao);
        }

        [HttpGet("historico")]
        public async Task<IActionResult> GetHistorico(
            [FromQuery] int faseId,
            [FromQuery] int pecaId)
        {
            var historico = await _registosProducaoService.GetHistoricoAsync(faseId, pecaId);
            return Ok(historico);
        }

        [HttpPost("add-registos_producao")]
        public async Task<IActionResult> AddRegistos_producao([FromBody] CreateRegistosProducaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var rp = new RegistosProducao
            {
                Fase_id = dto.Fase_id,
                Operador_id = dto.Operador_id,
                Peca_id = dto.Peca_id,
                Maquina_id = dto.Maquina_id,
                Estado_producao = dto.Estado_producao
            };

            var createdRegistoProducao = await _registosProducaoService.CreateRegistoProducaoAsync(rp);
            return CreatedAtAction(nameof(GetRegistos_producaoById), new { id = createdRegistoProducao.Registo_Producao_id }, createdRegistoProducao);
        }

        [HttpDelete("delete-registos_producao")]
        public async Task<IActionResult> DeleteRegistos_producao(int id)
        {
            await _registosProducaoService.DeleteRegistoProducaoAsync(id);
            return NoContent();
        }
    }

}
