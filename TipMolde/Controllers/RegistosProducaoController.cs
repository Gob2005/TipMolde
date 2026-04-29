using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.Dtos.RegistoProducaoDto;
using TipMolde.Application.Interface.Producao.IRegistosProducao;
using TipMolde.Domain.Entities.Producao;

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

        [Authorize(Roles = "ADMIN")]
        [HttpGet("all-registos_producao")]
        public async Task<IActionResult> GetAllRegistos_producao()
        {
            var result = await _registosProducaoService.GetAllAsync();
            return Ok(new
            {
                result.TotalCount,
                result.CurrentPage,
                result.PageSize,
                Items = result.Items
            });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("registos_producao-byID")]
        public async Task<IActionResult> GetRegistos_producaoById(int id)
        {
            var registoProducao = await _registosProducaoService.GetByIdAsync(id);
            if (registoProducao == null) return NotFound();
            return Ok(registoProducao);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("historico")]
        public async Task<IActionResult> GetHistorico(
            [FromQuery] int faseId,
            [FromQuery] int pecaId)
        {
            var historico = await _registosProducaoService.GetHistoricoAsync(faseId, pecaId);
            return Ok(historico);
        }

        [Authorize(Roles = "ADMIN, GESTOR_PRODUCAO")]
        [HttpPost("add-registos_producao")]
        public async Task<IActionResult> AddRegistos_producao([FromBody] CreateRegistosProducaoDto dto)
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

            var createdRegistoProducao = await _registosProducaoService.CreateAsync(rp);
            return CreatedAtAction(nameof(GetRegistos_producaoById), new { id = createdRegistoProducao.Registo_Producao_id }, createdRegistoProducao);
        }
    }

}
