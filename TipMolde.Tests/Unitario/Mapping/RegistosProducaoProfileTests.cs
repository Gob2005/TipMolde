using AutoMapper;
using FluentAssertions;
using TipMolde.Application.Dtos.RegistoProducaoDto;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Mapping;

/// <summary>
/// Testes unitarios do profile AutoMapper de RegistosProducao.
/// </summary>
[TestFixture]
[Category("Unit")]
public class RegistosProducaoProfileTests
{
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<RegistosProducaoProfile>());
        _mapper = config.CreateMapper();
    }

    [Test(Description = "TRPMAP001 - Configuracao AutoMapper de RegistosProducaoProfile deve ser valida.")]
    public void MappingConfiguration_Should_BeValid()
    {
        // ARRANGE
        var config = new MapperConfiguration(cfg => cfg.AddProfile<RegistosProducaoProfile>());

        // ACT
        Action act = () => config.AssertConfigurationIsValid();

        // ASSERT
        act.Should().NotThrow();
    }

    [Test(Description = "TRPMAP002 - CreateRegistosProducaoDto deve mapear para entidade sem definir campos geridos pelo servidor.")]
    public void CreateDto_Should_MapTo_Entity()
    {
        // ARRANGE
        var source = new CreateRegistosProducaoDto
        {
            Fase_id = 2,
            Operador_id = 3,
            Peca_id = 4,
            Maquina_id = 5,
            Estado_producao = EstadoProducao.PREPARACAO
        };

        // ACT
        var result = _mapper.Map<RegistosProducao>(source);

        // ASSERT
        result.Registo_Producao_id.Should().Be(0);
        result.Data_hora.Should().Be(default);
        result.Fase_id.Should().Be(2);
        result.Operador_id.Should().Be(3);
        result.Peca_id.Should().Be(4);
        result.Maquina_id.Should().Be(5);
        result.Estado_producao.Should().Be(EstadoProducao.PREPARACAO);
    }

    [Test(Description = "TRPMAP003 - Entidade deve mapear para ResponseRegistosProducaoDto.")]
    public void Entity_Should_MapTo_ResponseDto()
    {
        // ARRANGE
        var source = new RegistosProducao
        {
            Registo_Producao_id = 7,
            Fase_id = 2,
            Operador_id = 3,
            Peca_id = 4,
            Maquina_id = 5,
            Estado_producao = EstadoProducao.CONCLUIDO,
            Data_hora = DateTime.UtcNow
        };

        // ACT
        var result = _mapper.Map<ResponseRegistosProducaoDto>(source);

        // ASSERT
        result.Registo_Producao_id.Should().Be(7);
        result.Fase_id.Should().Be(2);
        result.Operador_id.Should().Be(3);
        result.Peca_id.Should().Be(4);
        result.Maquina_id.Should().Be(5);
        result.Estado_producao.Should().Be(EstadoProducao.CONCLUIDO);
        result.Data_hora.Should().Be(source.Data_hora);
    }
}
