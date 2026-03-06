using Microsoft.AspNetCore.Mvc;
using TipMolde.API.DTOs.MoldeDTO;
using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Models;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoldeController : ControllerBase
    {
        private readonly IMoldeService _moldeService;

        public MoldeController(IMoldeService moldeService)
        {
            _moldeService = moldeService;
        }

        [HttpGet("all-moldes")]
        public async Task<IActionResult> GetAllMoldes()
        {
            var moldes = await _moldeService.GetAllMoldesAsync();
            return Ok(moldes);
        }

        [HttpGet("molde-byID")]
        public async Task<IActionResult> GetMoldeById(int id)
        {
            var molde = await _moldeService.GetMoldeByIdAsync(id);
            if (molde == null) return NotFound();
            return Ok(molde);
        }

        [HttpGet("cliente-molde-byID")]
        public async Task<IActionResult> GetByClienteId(int clienteId)
        {
            var cliente = await _moldeService.GetClienteByIdAsync(clienteId);
            if (cliente == null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        [HttpPost("create-molde")]
        public async Task<IActionResult> CreateMolde([FromBody] CreateMoldeDTO dto)
        {
            if (dto == null) return BadRequest("Dados do molde sao obrigatorios.");
            if (dto.ClienteId <= 0) return BadRequest("O ID do cliente deve ser um numero positivo.");
            var cliente = await _moldeService.GetClienteByIdAsync(dto.ClienteId);
            if (cliente == null) return BadRequest("O cliente informado nao existe.");
            if (string.IsNullOrWhiteSpace(dto.Dimensoes_molde)) return BadRequest("As dimensoes do molde sao obrigatorias.");
            if (dto.Numero_cavidades <= 0) return BadRequest("O numero de cavidades deve ser um numero positivo.");

            var molde = new Molde
            {
                Cliente = cliente,
                Material = dto.Material,
                Dimensoes_molde = dto.Dimensoes_molde,
                Peso_estimado = dto.Peso_estimado,
                Numero_cavidades = dto.Numero_cavidades
            };
            var createdMolde = await _moldeService.CreateMoldeAsync(molde);

            var res = ToResponse(createdMolde);
            return CreatedAtAction(nameof(GetMoldeById), new { id = createdMolde.Molde_id }, res);
        }

        [HttpPut("update-molde")]
        public async Task<IActionResult> UpdateMolde(int id, [FromBody] UpdateMoldeDTO dto)
        {
            var molde = await _moldeService.GetMoldeByIdAsync(id);
            if (molde == null) return NotFound();

            molde.Material = !string.IsNullOrWhiteSpace(dto.Material) ? dto.Material.Trim() : molde.Material;
            molde.Dimensoes_molde = !string.IsNullOrWhiteSpace(dto.Dimensoes_molde) ? dto.Dimensoes_molde.Trim() : molde.Dimensoes_molde;
            molde.Peso_estimado = dto.Peso_estimado.HasValue && dto.Peso_estimado.Value > 0 ? dto.Peso_estimado.Value : molde.Peso_estimado;
            molde.Numero_cavidades = dto.Numero_cavidades.HasValue && dto.Numero_cavidades.Value > 0 ? dto.Numero_cavidades.Value : molde.Numero_cavidades;

            await _moldeService.UpdateMoldeAsync(molde);
            return NoContent();
        }

        [HttpDelete("delete-molde")]
        public async Task<IActionResult> DeleteMolde(int id)
        {
            var molde = await _moldeService.GetMoldeByIdAsync(id);
            if (molde == null) return NotFound();
            await _moldeService.DeleteMoldeAsync(id);
            return NoContent();
        }

        private static ResponseMoldeDTO ToResponse(Molde m) => new()
        {
            MoldeId = m.Molde_id,
            ClienteId = m.Cliente?.Cliente_id ?? 0,
            Material = m.Material,
            Dimensoes_molde = m.Dimensoes_molde,
            Peso_estimado = m.Peso_estimado,
            Numero_cavidades = m.Numero_cavidades
        };
    }
}

