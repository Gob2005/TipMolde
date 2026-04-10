using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.DTOs.RevisaoDTO;
using TipMolde.Application.Interface.Desenho.IRevisao;
using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RevisaoController : ControllerBase
    {
        private readonly IRevisaoService _service;

        public RevisaoController(IRevisaoService service)
        {
            _service = service;
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("by-projeto")]
        public async Task<IActionResult> GetByProjeto(int projetoId) =>
            Ok(await _service.GetByProjetoIdAsync(projetoId));

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpGet("by-id")]
        public async Task<IActionResult> GetById(int id)
        {
            var revisao = await _service.GetByIdAsync(id);
            if (revisao == null) return NotFound();
            return Ok(revisao);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateRevisaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var revisao = new Revisao
            {
                Projeto_id = dto.Projeto_id,
                DescricaoAlteracoes = dto.DescricaoAlteracoes.Trim()
            };

            var created = await _service.CreateAsync(revisao);
            return CreatedAtAction(nameof(GetById), new { id = created.Revisao_id }, created);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("resposta-cliente/{id:int}")]
        public async Task<IActionResult> UpdateRespostaCliente(int id, [FromBody] UpdateRespostaRevisaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _service.UpdateRespostaClienteAsync(id, dto.Aprovado, dto.FeedbackTexto, dto.FeedbackImagemPath);
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
