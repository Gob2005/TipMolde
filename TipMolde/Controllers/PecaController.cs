using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using TipMolde.Application.DTOs.PecaDTO;
using TipMolde.Application.Interface.Producao.IPeca;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PecaController : ControllerBase
    {
        private readonly IPecaService _pecaService;

        public PecaController(IPecaService pecaService)
        {
            _pecaService = pecaService;
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("all-pecas")]
         public async Task<IActionResult> GetAllPecas()
        {
            var result = await _pecaService.GetAllAsync();
            return Ok(new
            {
                result.TotalCount,
                result.CurrentPage,
                result.PageSize,
                Items = result.Items.Select(ToResponse)
            });
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("peca-byID")]
        public async Task<IActionResult> GetPecaById(int id)
        {
            var peca = await _pecaService.GetByIdAsync(id);
            if (peca == null) return NotFound();
            return Ok(ToResponse(peca));
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("by-molde")]
        public async Task<IActionResult> GetByMoldeId([FromQuery] int moldeId)
        {
            var pecas = await _pecaService.GetByMoldeIdAsync(moldeId);
            return Ok(pecas.Select(ToResponse));
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("by-designacao")]
        public async Task<IActionResult> GetByDesignacao([FromQuery] string designacao, [FromQuery] int moldeId)
        {
            if (string.IsNullOrWhiteSpace(designacao))
                return BadRequest("Designacao e obrigatoria.");

            var peca = await _pecaService.GetByDesignacaoAsync(designacao.Trim(), moldeId);
            if (peca == null) return NotFound();
            return Ok(ToResponse(peca));
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpPost("create-peca")]
        public async Task<IActionResult> CreatePeca([FromBody] CreatePecaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var peca = new Peca
            {
                Designacao = dto.Designacao.Trim(),
                Prioridade = dto.Prioridade,
                MaterialDesignacao = dto.MaterialDesignacao,
                MaterialRecebido = dto.MaterialRecebido,
                Molde_id = dto.Molde_id
            };

            var created = await _pecaService.CreateAsync(peca);
            return CreatedAtAction(nameof(GetPecaById), new { id = created.Peca_id }, created);
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpPut("update-peca/{id:int}")]
        public async Task<IActionResult> UpdatePeca(int id, [FromBody] UpdatePecaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var peca = new Peca
            {
                Peca_id = id,
                Designacao = dto.Designacao ?? string.Empty,
                Prioridade = dto.Prioridade ?? 0,
                MaterialDesignacao = dto.MaterialDesignacao,
                MaterialRecebido = dto.MaterialRecebido ?? false
            };

            await _pecaService.UpdateAsync(peca);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpDelete("delete-peca/{id:int}")]
        public async Task<IActionResult> DeletePeca(int id)
        {
            await _pecaService.DeleteAsync(id);
            return NoContent();
        }

        private static ResponsePecaDTO ToResponse(Peca p) => new()
        {
            Designacao = p.Designacao,
            Prioridade = p.Prioridade,
            MaterialDesignacao = p.MaterialDesignacao,
            MaterialRecebido = p.MaterialRecebido,
            MoldeId = p.Molde_id
        };
    }
}
