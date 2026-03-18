using Microsoft.AspNetCore.Mvc;
using TipMolde.API.DTOs.PecaDTO;
using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Interface.IPeca;
using TipMolde.Core.Models;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PecaController : ControllerBase
    {
        private readonly IPecaService _pecaService;
        private readonly IMoldeService _moldeService;

        public PecaController(IPecaService pecaService, IMoldeService moldeService)
        {
            _pecaService = pecaService;
            _moldeService = moldeService;
        }

         [HttpGet("all-pecas")]
         public async Task<IActionResult> GetAllPecas()
            {
                var pecas = await _pecaService.GetAllPecasAsync();
                return Ok(pecas);
        }

        [HttpGet("peca-byID")]
        public async Task<IActionResult> GetPecaById(int id)
        {
            var peca = await _pecaService.GetPecaByIdAsync(id);
            if (peca == null) return NotFound();
            return Ok(peca);
        }

        [HttpPost("create-peca")]
        public async Task<IActionResult> CreatePeca([FromBody] CreatePecaDTO dto)
        {
            var peca = new Peca
            {
                Numero_peca = dto.Numero_peca,
                Prioridade = dto.Prioridade,
                Descricao = dto.Descricao,
                Molde_id = dto.Molde_id
            };
            var createdPeca = await _pecaService.CreatePecaAsync(peca);
            return CreatedAtAction(nameof(GetPecaById), new { id = createdPeca.Peca_id }, createdPeca);
        }

        [HttpPut("update-peca/{id:int}")]
        public async Task<IActionResult> UpdatePeca(int id, [FromBody] UpdatePecaDTO dto)
        {
            var peca = await _pecaService.GetPecaByIdAsync(id);
            if (peca == null) return NotFound();

            peca.Numero_peca = dto.Numero_peca > 0 ? dto.Numero_peca : peca.Numero_peca;
            peca.Prioridade = dto.Prioridade > 1 ? dto.Prioridade : peca.Prioridade;
            peca.Descricao = dto.Descricao?.Trim() ?? peca.Descricao;

            await _pecaService.UpdatePecaAsync(peca);
            return NoContent();
        }

        [HttpDelete("delete-peca/{id:int}")]
        public async Task<IActionResult> DeletePeca(int id)
        {
            var peca = await _pecaService.GetPecaByIdAsync(id);
            if (peca == null) return NotFound();
            await _pecaService.DeletePecaAsync(id);
            return NoContent();
        }

        [HttpGet("search-number")]
        public async Task<IActionResult> SearchByNumber([FromQuery] int peca_id, [FromQuery] int molde_id)
        {
            var pecas = await _pecaService.GetPecaByNumberAsync(peca_id, molde_id);
            return Ok(pecas);
        }
    }
}
