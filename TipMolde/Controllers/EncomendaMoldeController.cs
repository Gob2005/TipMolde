using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.DTOs.EncomendaMoldeDTO;
using TipMolde.Application.Interface.Comercio.IEncomendaMolde;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EncomendaMoldeController : ControllerBase
    {
        private readonly IEncomendaMoldeService _service;

        public EncomendaMoldeController(IEncomendaMoldeService service)
        {
            _service = service;
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("by-encomenda")]
        public async Task<IActionResult> GetByEncomendaId(int encomendaId)
        {
            var links = await _service.GetByEncomendaIdAsync(encomendaId);
            return Ok(links);
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("by-molde")]
        public async Task<IActionResult> GetByMoldeId(int moldeId)
        {
            var links = await _service.GetByMoldeIdAsync(moldeId);
            return Ok(links);
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateEncomendaMoldeDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var link = new EncomendaMolde
            {
                Encomenda_id = dto.Encomenda_id,
                Molde_id = dto.Molde_id,
                Quantidade = dto.Quantidade,
                Prioridade = dto.Prioridade,
                DataEntregaPrevista = dto.DataEntregaPrevista
            };

            var created = await _service.CreateAsync(link);
            return Ok(created);
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEncomendaMoldeDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var link = new EncomendaMolde
            {
                EncomendaMolde_id = id,
                Quantidade = dto.Quantidade ?? 0,
                Prioridade = dto.Prioridade ?? 0,
                DataEntregaPrevista = dto.DataEntregaPrevista ?? default
            };

            await _service.UpdateAsync(link);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}

