using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.API.DTOs.MoldeDTO;
using TipMolde.Core.Interface.Producao.IMolde;
using TipMolde.Core.Models.Producao;
using TipMolde.Core.Models.Comercio;
using TipMolde.Core.Interface.Relatorios;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoldeController : ControllerBase
    {
        private readonly IMoldeService _moldeService;
        private readonly IRelatorioService _relatorioService;

        public MoldeController(IMoldeService moldeService, IRelatorioService relatorioService)
        {
            _moldeService = moldeService;
            _relatorioService = relatorioService;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("all-moldes")]
        public async Task<IActionResult> GetAllMoldes()
        {
            var moldes = await _moldeService.GetAllMoldesAsync();
            return Ok(moldes.Select(ToResponse));
        }

        [Authorize]
        [HttpGet("molde-byID")]
        public async Task<IActionResult> GetMoldeById(int id)
        {
            var molde = await _moldeService.GetMoldeWithSpecsAsync(id);
            if (molde == null) return NotFound();
            return Ok(ToResponse(molde));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("by-encomenda")]
        public async Task<IActionResult> GetByEncomendaId(int encomendaId)
        {
            var moldes = await _moldeService.GetByEncomendaIdAsync(encomendaId);
            return Ok(moldes.Select(ToResponse));
        }

        [Authorize]
        [HttpGet("by-numero")]
        public async Task<IActionResult> GetByNumero([FromQuery] string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                return BadRequest("Numero do molde e obrigatorio.");

            var molde = await _moldeService.GetByNumeroAsync(numero.Trim());
            if (molde == null) return NotFound();
            return Ok(ToResponse(molde));
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/ciclo-vida-pdf")]
        public async Task<IActionResult> ExportCicloVidaPdf(int id)
        {
            var result = await _relatorioService.GerarCicloVidaMoldePdfAsync(id);
            return File(result.Content, "application/pdf", result.FileName);
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPost("create-molde")]
        public async Task<IActionResult> CreateMolde([FromBody] CreateMoldeDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Numero))
                return BadRequest("Numero do molde e obrigatorio.");

            var molde = new Molde
            {
                Numero = dto.Numero.Trim(),
                NumeroMoldeCliente = dto.NumeroMoldeCliente,
                Nome = dto.Nome,
                ImagemCapaPath = dto.ImagemCapaPath,
                Descricao = dto.Descricao,
                Numero_cavidades = dto.Numero_cavidades,
                TipoPedido = dto.TipoPedido
            };

            var specs = new EspecificacoesTecnicas
            {
                Largura = dto.Largura,
                Comprimento = dto.Comprimento,
                Altura = dto.Altura,
                PesoEstimado = dto.PesoEstimado,
                TipoInjecao = dto.TipoInjecao,
                SistemaInjecao = dto.SistemaInjecao,
                Contracao = dto.Contracao,
                AcabamentoPeca = dto.AcabamentoPeca,
                Cor = dto.Cor,
                MaterialMacho = dto.MaterialMacho,
                MaterialCavidade = dto.MaterialCavidade,
                MaterialMovimentos = dto.MaterialMovimentos,
                MaterialInjecao = dto.MaterialInjecao
            };

            var link = new EncomendaMolde
            {
                Encomenda_id = dto.EncomendaId,
                Quantidade = dto.Quantidade,
                Prioridade = dto.Prioridade,
                DataEntregaPrevista = dto.DataEntregaPrevista
            };

            var created = await _moldeService.CreateMoldeAsync(molde, specs, link);
            return CreatedAtAction(nameof(GetMoldeById), new { id = created.Molde_id }, ToResponse(created));
        }

        [Authorize(Roles = "ADMIN,GESTOR_DESENHO")]
        [HttpPut("update-molde/{id:int}")]
        public async Task<IActionResult> UpdateMolde(int id, [FromBody] UpdateMoldeDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var molde = new Molde
            {
                Molde_id = id,
                Numero = dto.Numero ?? string.Empty,
                Nome = dto.Nome,
                ImagemCapaPath = dto.ImagemCapaPath,
                Descricao = dto.Descricao,
                Numero_cavidades = dto.Numero_cavidades ?? 0,
                TipoPedido = dto.TipoPedido ?? default
            };

            var specs = new EspecificacoesTecnicas
            {
                Largura = dto.Largura,
                Comprimento = dto.Comprimento,
                Altura = dto.Altura,
                PesoEstimado = dto.PesoEstimado,
                TipoInjecao = dto.TipoInjecao,
                SistemaInjecao = dto.SistemaInjecao,
                Contracao = dto.Contracao,
                AcabamentoPeca = dto.AcabamentoPeca,
                Cor = dto.Cor,
                MaterialMacho = dto.MaterialMacho,
                MaterialCavidade = dto.MaterialCavidade,
                MaterialMovimentos = dto.MaterialMovimentos,
                MaterialInjecao = dto.MaterialInjecao
            };

            await _moldeService.UpdateMoldeAsync(molde, specs);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete-molde/{id:int}")]
        public async Task<IActionResult> DeleteMolde(int id)
        {
            await _moldeService.DeleteMoldeAsync(id);
            return NoContent();
        }

        private static ResponseMoldeDTO ToResponse(Molde m) => new()
        {
            MoldeId = m.Molde_id,
            Numero = m.Numero,
            Nome = m.Nome,
            ImagemCapaPath = m.ImagemCapaPath,
            Descricao = m.Descricao,
            Numero_cavidades = m.Numero_cavidades,
            TipoPedido = m.TipoPedido,
            Largura = m.Especificacoes?.Largura,
            Comprimento = m.Especificacoes?.Comprimento,
            Altura = m.Especificacoes?.Altura,
            PesoEstimado = m.Especificacoes?.PesoEstimado,
            TipoInjecao = m.Especificacoes?.TipoInjecao,
            SistemaInjecao = m.Especificacoes?.SistemaInjecao,
            Contracao = m.Especificacoes?.Contracao,
            AcabamentoPeca = m.Especificacoes?.AcabamentoPeca,
            Cor = m.Especificacoes?.Cor,
            MaterialMacho = m.Especificacoes?.MaterialMacho,
            MaterialCavidade = m.Especificacoes?.MaterialCavidade,
            MaterialMovimentos = m.Especificacoes?.MaterialMovimentos,
            MaterialInjecao = m.Especificacoes?.MaterialInjecao
        };
    }
}

