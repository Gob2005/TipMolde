using AutoMapper;
using FluentAssertions;
using TipMolde.Application.Dtos.FasesProducaoDto;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Mapping;

[TestFixture]
[Category("Unit")]
public class FasesProducaoProfileTests
{
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<FasesProducaoProfile>());
        _mapper = config.CreateMapper();
    }

    [Test(Description = "TFPMAP1 - Configuracao AutoMapper de FasesProducaoProfile deve ser valida.")]
    public void MappingConfiguration_Should_BeValid()
    {
        // ARRANGE
        var config = new MapperConfiguration(cfg => cfg.AddProfile<FasesProducaoProfile>());

        // ACT
        Action act = () => config.AssertConfigurationIsValid();

        // ASSERT
        act.Should().NotThrow();
    }

    [Test(Description = "TFPMAP2 - CreateFasesProducaoDto deve mapear para entidade com trim nos campos de texto.")]
    public void CreateDto_Should_MapTo_Entity_WithTrimmedFields()
    {
        // ARRANGE
        var source = new CreateFasesProducaoDto
        {
            Nome = NomeFases.MAQUINACAO,
            Descricao = "  Descricao  "
        };

        // ACT
        var result = _mapper.Map<FasesProducao>(source);

        // ASSERT
        result.Nome.Should().Be(NomeFases.MAQUINACAO);
        result.Descricao.Should().Be("Descricao");
    }

    [Test(Description = "TFPMAP3 - Entidade deve mapear para ResponseFasesProducaoDto.")]
    public void Entity_Should_MapTo_ResponseDto()
    {
        // ARRANGE
        var source = new FasesProducao
        {
            Fases_producao_id = 9,
            Nome = NomeFases.EROSAO,
            Descricao = "Descricao"
        };

        // ACT
        var result = _mapper.Map<ResponseFasesProducaoDto>(source);

        // ASSERT
        result.FasesProducao_id.Should().Be(9);
        result.Nome.Should().Be(NomeFases.EROSAO);
        result.Descricao.Should().Be("Descricao");
    }

    [Test(Description = "TFPMAP4 - UpdateFasesProducaoDto deve atualizar apenas os campos enviados.")]
    public void UpdateDto_Should_MapOnlyProvidedFields()
    {
        // ARRANGE
        var source = new UpdateFasesProducaoDto
        {
            Descricao = "  Nova descricao  "
        };

        var destination = new FasesProducao
        {
            Fases_producao_id = 2,
            Nome = NomeFases.MONTAGEM,
            Descricao = "Descricao antiga"
        };

        // ACT
        _mapper.Map(source, destination);

        // ASSERT
        destination.Nome.Should().Be(NomeFases.MONTAGEM);
        destination.Descricao.Should().Be("Nova descricao");
    }
}
