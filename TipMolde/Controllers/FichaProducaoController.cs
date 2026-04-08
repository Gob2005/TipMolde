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
        [HttpGet("{id:int}/export-pdf")]
        public async Task<IActionResult> ExportPdf(int id)
        {
            var result = await _relatorioService.GerarFichaPdfFTLAsync(id);
            return File(result.Content, "application/pdf", result.FileName);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/export-excel")]
        public async Task<IActionResult> ExportExcel(int id)
        {
            var result = await _relatorioService.GerarFichaExcelFTLAsync(id);
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
        [HttpPost("{id:int}/ocorrencias")]
        public async Task<IActionResult> AddOcorrencia(int id, [FromBody] CreateRegistoOcorrenciaDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entidade = new RegistoOcorrencia
            {
                Descricao = dto.Descricao.Trim(),
                Correcao = dto.Correcao,
                Responsavel_id = dto.Responsavel_id
            };

            var created = await _service.AddOcorrenciaAsync(id, entidade);
            return Ok(created);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("{id:int}/melhorias-alteracoes")]
        public async Task<IActionResult> AddMelhoriaAlteracao(int id, [FromBody] CreateRegistoMelhoriaAlteracaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entidade = new RegistoMelhoriaAlteracao
            {
                ItemDescricao = dto.ItemDescricao.Trim(),
                Pormenor = dto.Pormenor,
                Verificado = dto.Verificado,
                Responsavel_id = dto.Responsavel_id
            };

            var created = await _service.AddMelhoriaAlteracaoAsync(id, entidade);
            return Ok(created);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id:int}/ensaio")]
        public async Task<IActionResult> UpsertEnsaio(int id, [FromBody] UpsertRegistoEnsaioDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entidade = new RegistoEnsaio
            {
                LocalEnsaio = dto.LocalEnsaio.Trim(),
                AguasCavidade = dto.AguasCavidade,
                AguasMacho = dto.AguasMacho,
                AguasMovimentos = dto.AguasMovimentos,
                ResumoTexto = dto.ResumoTexto,
                Maquina_id = dto.Maquina_id,
                Responsavel_id = dto.Responsavel_id
            };

            var saved = await _service.UpsertEnsaioAsync(id, entidade);
            return Ok(saved);
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
