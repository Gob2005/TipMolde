using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Core.Interface.Fichas.IFichaDocumento;

namespace TipMolde.Controllers
{
    [ApiController]
    [Route("api/fichas")]
    [Authorize]
    public class FichaDocumentoController : ControllerBase
    {
        private readonly IFichaDocumentoService _service;

        public FichaDocumentoController(IFichaDocumentoService service)
        {
            _service = service;
        }

        [HttpPost("{fichaId:int}/documentos/upload")]
        public async Task<IActionResult> Upload(int fichaId, IFormFile file)
        {
            var userIdClaim = User.FindFirst("id")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Utilizador invalido no token.");
            var doc = await _service.UploadAsync(fichaId, file, userId);
            return Ok(doc);
        }

        [HttpGet("{fichaId:int}/documentos")]
        public async Task<IActionResult> Listar(int fichaId)
        {
            var docs = await _service.ListarAsync(fichaId);
            return Ok(docs);
        }

        [HttpGet("documentos/{documentoId:int}/download")]
        public async Task<IActionResult> Download(int documentoId)
        {
            var result = await _service.DownloadAsync(documentoId);
            return File(result.Content, result.TipoFicheiro, result.FileName);
        }
    }
}
