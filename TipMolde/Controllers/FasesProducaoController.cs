using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.DTOs.Fases_producaoDTO;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FasesProducaoController : ControllerBase
    {
        private readonly IFasesProducaoService _fasesProducaoService;

        public FasesProducaoController(IFasesProducaoService fasesProducaoService)
        {
            _fasesProducaoService = fasesProducaoService;
        }

        [Authorize(Roles = "ADMIN,GESTOR_PRODUCAO")]
        [HttpGet("all-fases_producao")]
        public async Task<IActionResult> GetAllFases_producao()
        {
            var fasesProducao = await _fasesProducaoService.GetAllAsync();
            return Ok(fasesProducao);
        }

        [Authorize(Roles = "ADMIN,GESTOR_PRODUCAO")]
        [HttpGet("fases_producao-byID")]
        public async Task<IActionResult> GetFases_producaoById(int id)
        {
            var fasesProducao = await _fasesProducaoService.GetByIdAsync(id);
            if (fasesProducao == null) return NotFound();
            return Ok(fasesProducao);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("create-fases_producao")]
        public async Task<IActionResult> CreateFases_producao([FromBody] CreateFasesProducaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var fasesProducao = new FasesProducao
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao
            };

            var created = await _fasesProducaoService.CreateAsync(fasesProducao);
            return CreatedAtAction(nameof(GetFases_producaoById), new { id = created.Fases_producao_id }, created);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("update-fases_producao/{id:int}")]
        public async Task<IActionResult> UpdateFases_producao(int id, [FromBody] UpdateFasesProducaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var fase = await _fasesProducaoService.GetByIdAsync(id);
            if (fase == null) return NotFound();

            if (dto.Nome.HasValue) fase.Nome = dto.Nome.Value;
            if (!string.IsNullOrWhiteSpace(dto.Descricao)) fase.Descricao = dto.Descricao;

            await _fasesProducaoService.UpdateAsync(fase);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete-fases_producao/{id:int}")]
        public async Task<IActionResult> DeleteFases_producao(int id)
        {
            var fasesProducao = await _fasesProducaoService.GetByIdAsync(id);
            if (fasesProducao == null) return NotFound();

            await _fasesProducaoService.DeleteAsync(id);
            return NoContent();
        }
    }
}