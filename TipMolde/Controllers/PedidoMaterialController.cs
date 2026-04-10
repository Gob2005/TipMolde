using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using TipMolde.Application.DTOs.PedidoMaterialDTO;
using TipMolde.Application.Interface.Comercio.IPedidoMaterial;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoMaterialController : ControllerBase
    {
        private readonly IPedidoMaterialService _service;

        public PedidoMaterialController(IPedidoMaterialService service)
        {
            _service = service;
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(new
            {
                result.TotalCount,
                result.CurrentPage,
                result.PageSize,
                Items = result.Items.Select(ToResponse)
            });
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("by-id")]
        public async Task<IActionResult> GetById(int id)
        {
            var pedido = await _service.GetByIdAsync(id);
            if (pedido == null) return NotFound();
            return Ok(ToResponse(pedido));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("with-itens")]
        public async Task<IActionResult> GetWithItens(int id)
        {
            var pedido = await _service.GetWithItensAsync(id);
            if (pedido == null) return NotFound();
            return Ok(ToResponse(pedido));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpGet("by-fornecedor")]
        public async Task<IActionResult> GetByFornecedorId(int fornecedorId)
        {
            var pedidos = await _service.GetByFornecedorIdAsync(fornecedorId);
            return Ok(pedidos.Select(ToResponse));
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreatePedidoMaterialDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var pedido = new PedidoMaterial
            {
                Fornecedor_id = dto.Fornecedor_id
            };

            var itens = dto.Itens.Select(i => new ItemPedidoMaterial
            {
                Peca_id = i.Peca_id,
                Quantidade = i.Quantidade
            });

            var created = await _service.CreateAsync(pedido, itens);
            return CreatedAtAction(nameof(GetById), new { id = created.PedidoMaterial_id }, ToResponse(created));
        }

        [Authorize]
        [HttpPut("registar-rececao/{id:int}")]
        public async Task<IActionResult> RegistarRececao(int id, [FromBody] RegistarRececaoDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _service.RegistarRececaoAsync(id, dto.UserId);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN,GESTOR_COMERCIAL")]
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        private static ResponsePedidoMaterialDTO ToResponse(PedidoMaterial p) => new()
        {
            PedidoMaterialId = p.PedidoMaterial_id,
            DataPedido = p.DataPedido,
            DataRececao = p.DataRececao,
            Estado = p.Estado,
            FornecedorId = p.Fornecedor_id,
            UserConferenteId = p.UserConferente_id,
            Itens = p.Itens.Select(i => new ResponseItemPedidoMaterialDTO
            {
                ItemId = i.ItemPedidoMaterial_id,
                PecaId = i.Peca_id,
                Quantidade = i.Quantidade
            }).ToList()
        };
    }
}
