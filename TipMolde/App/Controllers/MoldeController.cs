using Microsoft.AspNetCore.Mvc;
using TipMolde.App.DTOs.MoldeDTO;
using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Models;

namespace TipMolde.App.Controllers
{
    public class MoldeController : ControllerBase
    {
        private readonly IMoldeService _moldeService;

        public MoldeController(IMoldeService moldeService)
        {
            _moldeService = moldeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMoldes()
        {
            var moldes = await _moldeService.GetAllMoldesAsync();
            return Ok(moldes);
        }

        [HttpGet]
        public async Task<IActionResult> GetMoldeById(int id)
        {
            var molde = await _moldeService.GetMoldeByIdAsync(id);
            if (molde == null) return NotFound();
            return Ok(molde);
        }

        [HttpGet]
        public async Task<IActionResult> GetClienteId(int clienteId)
        {
            var moldes = await _moldeService.GetClienteByIdAsync(clienteId);
            return Ok(moldes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMolde(CreateMoldeDTO dto)
        {
            if(dto == null) return BadRequest("Dados do molde sao obrigatorios.");
            if (dto.ClienteId <= 0) return BadRequest("O ID do cliente deve ser um numero positivo.");
            var cliente = await _moldeService.GetClienteByIdAsync(dto.ClienteId);
            if (cliente == null) return BadRequest("O cliente informado nao existe.");
            if (string.IsNullOrWhiteSpace(dto.Dimensoes_molde)) return BadRequest("As dimensões do molde sao obrigatorias.");
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

        [HttpPut]
        public async Task<IActionResult> UpdateMolde(int id, UpdateMoldeDTO dto)
        {
            var molde = await _moldeService.GetMoldeByIdAsync(id);
            if (molde == null) return NotFound();
            molde.Material = !string.IsNullOrWhiteSpace(dto.Material) ? dto.Material.Trim() : molde.Material;
            molde.Dimensoes_molde = !string.IsNullOrWhiteSpace(dto.Dimensoes_molde) ? dto.Dimensoes_molde.Trim() : molde.Dimensoes_molde;
            molde.Peso_estimado = dto.Peso_estimado > 0 ? dto.Peso_estimado : molde.Peso_estimado;
            molde.Numero_cavidades = dto.Numero_cavidades > 0 ? dto.Numero_cavidades : molde.Numero_cavidades;
            await _moldeService.UpdateMoldeAsync(molde);
            return NoContent();
        }

        [HttpDelete]
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
            ClienteId = m.Cliente.Cliente_id,
            Material = m.Material,
            Dimensoes_molde = m.Dimensoes_molde,
            Peso_estimado = m.Peso_estimado,
            Numero_cavidades = m.Numero_cavidades
        };
    }
}
