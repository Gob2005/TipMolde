using AutoMapper;
using FluentAssertions;
using TipMolde.Application.Dtos.PedidoMaterialDto;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Mapping;

[TestFixture]
[Category("Unit")]
public class PedidoMaterialProfileTests
{
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PedidoMaterialProfile>();
        });

        _mapper = config.CreateMapper();
    }

    [Test(Description = "TPMMAP1 - Configuracao do AutoMapper para PedidoMaterial e valida.")]
    public void MappingConfiguration_Should_BeValid_When_ProfileIsLoaded()
    {
        // ARRANGE
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PedidoMaterialProfile>();
        });

        // ACT
        Action act = () => config.AssertConfigurationIsValid();

        // ASSERT
        act.Should().NotThrow();
    }

    [Test(Description = "TPMMAP2 - CreatePedidoMaterialDto deve mapear linhas para o agregado de dominio.")]
    public void CreatePedidoMaterialDTO_Should_MapItens_When_DtoContainsLines()
    {
        // ARRANGE
        var dto = new CreatePedidoMaterialDto
        {
            Fornecedor_id = 10,
            Itens =
            {
                new CreateItemPedidoMaterialDto { Peca_id = 2, Quantidade = 5 }
            }
        };

        // ACT
        var result = _mapper.Map<PedidoMaterial>(dto);

        // ASSERT
        result.Fornecedor_id.Should().Be(10);
        result.Itens.Should().ContainSingle();
        result.Itens.Single().Peca_id.Should().Be(2);
        result.Itens.Single().Quantidade.Should().Be(5);
    }

    [Test(Description = "TPMMAP3 - PedidoMaterial deve mapear para DTO de resposta com itens coerentes.")]
    public void PedidoMaterial_Should_MapToResponseDto_When_EntityContainsItens()
    {
        // ARRANGE
        var entity = new PedidoMaterial
        {
            PedidoMaterial_id = 30,
            Fornecedor_id = 11,
            Estado = EstadoPedido.RECEBIDO,
            UserConferente_id = 7,
            DataPedido = new DateTime(2026, 4, 23, 10, 0, 0, DateTimeKind.Utc),
            DataRececao = new DateTime(2026, 4, 23, 11, 0, 0, DateTimeKind.Utc),
            Itens =
            {
                new ItemPedidoMaterial
                {
                    ItemPedidoMaterial_id = 1,
                    Peca_id = 100,
                    Quantidade = 4
                }
            }
        };

        // ACT
        var result = _mapper.Map<ResponsePedidoMaterialDto>(entity);

        // ASSERT
        result.PedidoMaterialId.Should().Be(30);
        result.FornecedorId.Should().Be(11);
        result.UserConferenteId.Should().Be(7);
        result.Estado.Should().Be(EstadoPedido.RECEBIDO);
        result.Itens.Should().ContainSingle();
        result.Itens.Single().ItemId.Should().Be(1);
        result.Itens.Single().PecaId.Should().Be(100);
        result.Itens.Single().Quantidade.Should().Be(4);
    }
}
