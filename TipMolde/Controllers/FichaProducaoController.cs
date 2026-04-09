using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.API.DTOs.FichaProducaoDTO;
using TipMolde.Core.Interface.Fichas.IFichaProducao;
using TipMolde.Core.Interface.Relatorios;
using TipMolde.Core.Models.Fichas;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FichaProducaoController : ControllerBase
    {
        private readonly IFichaProducaoService _service;
        private readonly IRelatorioService _relatorioService;

        public FichaProducaoController(IFichaProducaoService service, IRelatorioService relatorioService)
        {
            _service = service;
            _relatorioService = relatorioService;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("by-encomendamolde")]
        public async Task<IActionResult> GetByEncomendaMoldeId(int encomendaMoldeId) =>
            Ok(await _service.GetByEncomendaMoldeIdAsync(encomendaMoldeId));

        [Authorize(Roles = "ADMIN")]
        [HttpGet("header-by-id")]
        public async Task<IActionResult> GetHeaderById(int id)
        {
            var ficha = await _service.GetByIdWithHeaderAsync(id);
            if (ficha == null) return NotFound();
            return Ok(ficha);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("flt-by-id")]
        public async Task<IActionResult> GetFLTById(int id)
        {
            var ficha = await _service.GetFLTByIdAsync(id);
            if (ficha == null) return NotFound();
            return Ok(ficha);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/export-FLT")]
        public async Task<IActionResult> ExportFLT(int id)
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Utilizador invalido no token.");
            var result = await _relatorioService.GerarFichaExcelFLTAsync(id, userId);
            return File(result.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileName);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/export-FRE")]
        public async Task<IActionResult> ExportFRE(int id)
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Utilizador invalido no token.");
            var result = await _relatorioService.GerarFichaExcelFREAsync(id, userId);
            return File(result.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileName);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/export-FRM")]
        public async Task<IActionResult> ExportFRM(int id)
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Utilizador invalido no token.");
            var result = await _relatorioService.GerarFichaExcelFRMAsync(id, userId);
            return File(result.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileName);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/export-FRA")]
        public async Task<IActionResult> ExportFRA(int id)
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Utilizador invalido no token.");
            var result = await _relatorioService.GerarFichaExcelFRAAsync(id, userId);
            return File(result.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileName);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/export-FOP")]
        public async Task<IActionResult> ExportFOP(int id)
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Utilizador invalido no token.");
            var result = await _relatorioService.GerarFichaExcelFOPAsync(id, userId);
            return File(result.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.FileName);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateFichaProducaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ficha = new FichaProducao
            {
                Tipo = dto.Tipo,
                EncomendaMolde_id = dto.EncomendaMolde_id
            };

            var created = await _service.CreateAsync(ficha);
            return CreatedAtAction(nameof(GetHeaderById), new { id = created.FichaProducao_id }, created);
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
