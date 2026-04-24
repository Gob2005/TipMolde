using AutoMapper;
using FluentAssertions;
using TipMolde.Application.Dtos.PecaDto;
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

    [Test(Description = "TPECAMAP2 - Entidade Peca deve mapear para ResponsePecaDto com PecaId preenchido.")]
    public void Peca_Should_MapTo_ResponsePecaDto()
    {
        // ARRANGE
        var source = new Peca
        {
            Peca_id = 5,
            NumeroPeca = "100A",
            Designacao = "Extrator",
            Prioridade = 2,
            Quantidade = 3,
            Referencia = "REF-1",
            MaterialDesignacao = "Aco",
            TratamentoTermico = "Temperado",
            Massa = "0,20kg",
            Observacao = "34,92",
            MaterialRecebido = true,
            Molde_id = 7
        };

        // ACT
        var result = _mapper.Map<ResponsePecaDto>(source);

        // ASSERT
        result.PecaId.Should().Be(5);
        result.NumeroPeca.Should().Be("100A");
        result.Designacao.Should().Be("Extrator");
        result.Quantidade.Should().Be(3);
        result.Referencia.Should().Be("REF-1");
        result.TratamentoTermico.Should().Be("Temperado");
        result.Massa.Should().Be("0,20kg");
        result.Observacao.Should().Be("34,92");
        result.MaterialRecebido.Should().BeTrue();
        result.Molde_id.Should().Be(7);
    }

    [Test(Description = "TPECAMAP3 - CreatePecaDto deve mapear para Peca com normalizacao simples de strings.")]
    public void CreatePecaDTO_Should_MapTo_Peca()
    {
        // ARRANGE
        var source = new CreatePecaDto
        {
            NumeroPeca = "  100A  ",
            Designacao = "  Extrator  ",
            Prioridade = 3,
            Quantidade = 7,
            Referencia = "  REF-1  ",
            MaterialDesignacao = "  Inox  ",
            TratamentoTermico = "  Temperado  ",
            Massa = "  0,20kg  ",
            Observacao = "  34,92  ",
            MaterialRecebido = false,
            Molde_id = 11
        };

        // ACT
        var result = _mapper.Map<Peca>(source);

        // ASSERT
        result.NumeroPeca.Should().Be("100A");
        result.Designacao.Should().Be("Extrator");
        result.Prioridade.Should().Be(3);
        result.Quantidade.Should().Be(7);
        result.Referencia.Should().Be("REF-1");
        result.MaterialDesignacao.Should().Be("Inox");
        result.TratamentoTermico.Should().Be("Temperado");
        result.Massa.Should().Be("0,20kg");
        result.Observacao.Should().Be("34,92");
        result.MaterialRecebido.Should().BeFalse();
        result.Molde_id.Should().Be(11);
    }

    [Test(Description = "TPECAMAP4 - UpdatePecaDto deve atualizar apenas campos enviados e preservar os restantes.")]
    public void UpdatePecaDTO_Should_MapOnlyProvidedFields_When_MappingToExistingPeca()
    {
        // ARRANGE
        var source = new UpdatePecaDto
        {
            NumeroPeca = "  100A  ",
            Designacao = "  Nova Peca  ",
            Prioridade = 4,
            Quantidade = 5,
            Referencia = "  REF-2  ",
            TratamentoTermico = "  Revenido  ",
            Massa = "  0,30kg  ",
            Observacao = "  55  "
        };

        var destination = new Peca
        {
            Peca_id = 9,
            NumeroPeca = "090A",
            Designacao = "Peca Antiga",
            Prioridade = 1,
            Quantidade = 1,
            Referencia = "REF-1",
            MaterialDesignacao = "Aco",
            TratamentoTermico = "Temperado",
            Massa = "0,20kg",
            Observacao = "34,92",
            MaterialRecebido = true,
            Molde_id = 3
        };

        // ACT
        _mapper.Map(source, destination);

        // ASSERT
        destination.NumeroPeca.Should().Be("100A");
        destination.Designacao.Should().Be("Nova Peca");
        destination.Prioridade.Should().Be(4);
        destination.Quantidade.Should().Be(5);
        destination.Referencia.Should().Be("REF-2");
        destination.MaterialDesignacao.Should().Be("Aco");
        destination.TratamentoTermico.Should().Be("Revenido");
        destination.Massa.Should().Be("0,30kg");
        destination.Observacao.Should().Be("55");
        destination.MaterialRecebido.Should().BeTrue();
        destination.Molde_id.Should().Be(3);
    }
}
