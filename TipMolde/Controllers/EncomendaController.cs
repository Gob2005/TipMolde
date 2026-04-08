using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.API.DTOs.EncomendaDTO;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.Comercio.IEncomenda;
using TipMolde.Core.Interface.Comercio.ICliente;
using TipMolde.Core.Models.Comercio;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EncomendaController : ControllerBase
    {
        private readonly IEncomendaService _encomendaService;

        public EncomendaController(IEncomendaService encomendaService)
        {
            _encomendaService = encomendaService;
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("all-encomendas")]
        public async Task<IActionResult> GetAllEncomendas()
        {
            var encomendas = await _encomendaService.GetAllEncomendasAsync();
            return Ok(encomendas.Select(ToResponse));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("encomenda-byID")]
        public async Task<IActionResult> GetEncomendaById(int id)
        {
            var encomenda = await _encomendaService.GetEncomendaByIdAsync(id);
            if (encomenda == null) return NotFound();
            return Ok(ToResponse(encomenda));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("encomenda-with-moldes")]
        public async Task<IActionResult> GetEncomendaWithMoldes(int id)
        {
            var encomenda = await _encomendaService.GetEncomendaWithMoldesAsync(id);
            if (encomenda == null) return NotFound();
            return Ok(ToResponse(encomenda));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("por-concluir")]
        public async Task<IActionResult> GetEncomendasPorConcluir()
        {
            var encomendas = await _encomendaService.GetEncomendasPorConcluirAsync();
            return Ok(encomendas.Select(ToResponse));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("by-estado")]
        public async Task<IActionResult> GetByEstado(EstadoEncomenda estado)
        {
            var encomendas = await _encomendaService.GetByEstadoAsync(estado);
            return Ok(encomendas.Select(ToResponse));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("by-numero-cliente")]
        public async Task<IActionResult> GetByNumeroCliente([FromQuery] string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return BadRequest("O numero de encomenda do cliente e obrigatorio.");

            var encomenda = await _encomendaService.GetByNumeroEncomendaClienteAsync(numero);
            if (encomenda == null) return NotFound();
            return Ok(ToResponse(encomenda));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPost("create-encomenda")]
        public async Task<IActionResult> CreateEncomenda([FromBody] CreateEncomendaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var encomenda = new Encomenda
            {
                NumeroEncomendaCliente = dto.NumeroEncomendaCliente.Trim(),
                NumeroProjetoCliente = dto.NumeroProjetoCliente,
                NomeServicoCliente = dto.NomeServicoCliente,
                NomeResponsavelCliente = dto.NomeResponsavelCliente,
                Cliente_id = dto.Cliente_id
            };

            var created = await _encomendaService.CreateEncomendaAsync(encomenda);
            return CreatedAtAction(
                nameof(GetEncomendaById),
                new { id = created.Encomenda_id },
                ToResponse(created));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPut("update-encomenda")]
        public async Task<IActionResult> UpdateEncomenda(int id, [FromBody] UpdateEncomendaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var encomenda = new Encomenda
            {
                Encomenda_id = id,
                NumeroEncomendaCliente = dto.NumeroEncomendaCliente ?? string.Empty,
                NumeroProjetoCliente = dto.NumeroProjetoCliente,
                NomeServicoCliente = dto.NomeServicoCliente,
                NomeResponsavelCliente = dto.NomeResponsavelCliente
            };

            await _encomendaService.UpdateEncomendaAsync(encomenda);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPut("update-estado/{id:int}")]
        public async Task<IActionResult> UpdateEstado(int id, [FromBody] UpdateEstadoEncomendaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _encomendaService.UpdateEstadoEncomendaAsync(id, dto.Estado);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpDelete("delete-encomenda")]
        public async Task<IActionResult> DeleteEncomenda(int id)
        {
            var encomenda = await _encomendaService.GetEncomendaByIdAsync(id);
            if (encomenda == null) return NotFound();
            await _encomendaService.DeleteEncomendaAsync(id);
            return NoContent();
        }

        private static ResponseEncomendaDTO ToResponse(Encomenda e) => new()
        {
            Encomenda_id = e.Encomenda_id,
            NumeroEncomendaCliente = e.NumeroEncomendaCliente,
            NumeroProjetoCliente = e.NumeroProjetoCliente,
            NomeServicoCliente = e.NomeServicoCliente,
            NomeResponsavelCliente = e.NomeResponsavelCliente,
            DataRegisto = e.DataRegisto,
            Estado = e.Estado,
            Cliente_id = e.Cliente_id,
            NomeCliente = e.Cliente?.Nome
        };
    }
}
