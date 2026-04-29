using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TipMolde.Application.Dtos.FichaProducaoDto;
using TipMolde.Application.Interface.Fichas.IFichaProducao;
using TipMolde.Application.Interface.Relatorios;
using TipMolde.Domain.Entities.Fichas;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/fichas-producao")]
    public class RelatorioController : ControllerBase
    {
        private readonly IFichaProducaoService _service;
        private readonly IRelatorioService _relatorioService;
        private readonly ILogger<RelatorioController> _logger;
        public RelatorioController(
            IFichaProducaoService service,
            IRelatorioService relatorioService,
            ILogger<RelatorioController> logger)
        {
            _service = service;
            _relatorioService = relatorioService;
            _logger = logger;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/export/flt")]
        [HttpGet("{id:int}/export-FLT")]
        public Task<IActionResult> ExportFLT(int id) =>
            ExportFichaAsync(id, "FLT", _relatorioService.GerarFichaExcelFLTAsync);

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/export/fre")]
        [HttpGet("{id:int}/export-FRE")]
        public Task<IActionResult> ExportFRE(int id) =>
            ExportFichaAsync(id, "FRE", _relatorioService.GerarFichaExcelFREAsync);

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/export/frm")]
        [HttpGet("{id:int}/export-FRM")]
        public Task<IActionResult> ExportFRM(int id) =>
            ExportFichaAsync(id, "FRM", _relatorioService.GerarFichaExcelFRMAsync);

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/export/fra")]
        [HttpGet("{id:int}/export-FRA")]
        public Task<IActionResult> ExportFRA(int id) =>
            ExportFichaAsync(id, "FRA", _relatorioService.GerarFichaExcelFRAAsync);

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}/export/fop")]
        [HttpGet("{id:int}/export-FOP")]
        public Task<IActionResult> ExportFOP(int id) =>
            ExportFichaAsync(id, "FOP", _relatorioService.GerarFichaExcelFOPAsync);

        private async Task<IActionResult> ExportFichaAsync(
            int fichaId,
            string tipoFicha,
            Func<int, int, Task<(byte[] Content, string FileName)>> exportFunc)
        {
            if (!TryGetAuthenticatedUserId(out var userId, out var errorResult))
                return errorResult!;

            var result = await exportFunc(fichaId, userId);

            _logger.LogInformation(
                "Exportacao {TipoFicha} gerada para ficha {FichaId} pelo utilizador {UserId}",
                tipoFicha,
                fichaId,
                userId);

            return File(
                result.Content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                result.FileName);
        }

        private bool TryGetAuthenticatedUserId(out int userId, out IActionResult? errorResult)
        {
            var claimValue =
                User.FindFirstValue("id") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(claimValue, out userId))
            {
                errorResult = null;
                return true;
            }

            errorResult = Unauthorized(this.CreateProblem(
                StatusCodes.Status401Unauthorized,
                "Token invalido",
                "Nao foi possivel determinar o utilizador autenticado."));
            return false;
        }
    }
}
