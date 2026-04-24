using AutoMapper;
using FluentAssertions;
using TipMolde.Application.DTOs.PecaDTO;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Tests.Unitario.Mapping;

[TestFixture]
[Category("Unit")]
public class PecaProfileTests
{
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<PecaProfile>());
        _mapper = config.CreateMapper();
    }

    [Test(Description = "TPECAMAP1 - Configuracao AutoMapper de PecaProfile deve ser valida.")]
    public void MappingConfiguration_Should_BeValid()
    {
        // ARRANGE
        var config = new MapperConfiguration(cfg => cfg.AddProfile<PecaProfile>());

        // ACT
        Action act = () => config.AssertConfigurationIsValid();

        // ASSERT
        act.Should().NotThrow();
    }

    [Test(Description = "TPECAMAP2 - Entidade Peca deve mapear para ResponsePecaDTO com PecaId preenchido.")]
    public void Peca_Should_MapTo_ResponsePecaDTO()
    {
        // ARRANGE
        var source = new Peca
        {
            Peca_id = 5,
            Designacao = "Extrator",
            Prioridade = 2,
            MaterialDesignacao = "Aco",
            MaterialRecebido = true,
            Molde_id = 7
        };

        // ACT
        var result = _mapper.Map<ResponsePecaDTO>(source);

        // ASSERT
        result.PecaId.Should().Be(5);
        result.Designacao.Should().Be("Extrator");
        result.MaterialRecebido.Should().BeTrue();
        result.Molde_id.Should().Be(7);
    }

    [Test(Description = "TPECAMAP3 - CreatePecaDTO deve mapear para Peca com normalizacao simples de strings.")]
    public void CreatePecaDTO_Should_MapTo_Peca()
    {
        // ARRANGE
        var source = new CreatePecaDTO
        {
            Designacao = "  Extrator  ",
            Prioridade = 3,
            MaterialDesignacao = "  Inox  ",
            MaterialRecebido = false,
            Molde_id = 11
        };

        // ACT
        var result = _mapper.Map<Peca>(source);

        // ASSERT
        result.Designacao.Should().Be("Extrator");
        result.MaterialDesignacao.Should().Be("Inox");
        result.Molde_id.Should().Be(11);
    }

    [Test(Description = "TPECAMAP4 - UpdatePecaDTO deve atualizar apenas campos enviados e preservar os restantes.")]
    public void UpdatePecaDTO_Should_MapOnlyProvidedFields_When_MappingToExistingPeca()
    {
        // ARRANGE
        var source = new UpdatePecaDTO
        {
            Designacao = "  Nova Peca  ",
            Prioridade = 4
        };

        var destination = new Peca
        {
            Peca_id = 9,
            Designacao = "Peca Antiga",
            Prioridade = 1,
            MaterialDesignacao = "Aco",
            MaterialRecebido = true,
            Molde_id = 3
        };

        // ACT
        _mapper.Map(source, destination);

        // ASSERT
        destination.Designacao.Should().Be("Nova Peca");
        destination.Prioridade.Should().Be(4);
        destination.MaterialDesignacao.Should().Be("Aco");
        destination.MaterialRecebido.Should().BeTrue();
        destination.Molde_id.Should().Be(3);
    }
}
