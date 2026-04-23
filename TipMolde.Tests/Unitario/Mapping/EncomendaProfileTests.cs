using AutoMapper;
using FluentAssertions;
using TipMolde.Application.DTOs.EncomendaDTO;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Mapping;

[TestFixture]
[Category("Unit")]
public class EncomendaProfileTests
{
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EncomendaProfile>();
        });

        _mapper = config.CreateMapper();
    }

    [Test(Description = "TENCMP1 - Configuracao AutoMapper de EncomendaProfile deve ser valida.")]
    public void MappingConfiguration_Should_BeValid_When_EncomendaProfileIsLoaded()
    {
        // ARRANGE
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EncomendaProfile>();
        });

        // ACT
        Action act = () => config.AssertConfigurationIsValid();

        // ASSERT
        act.Should().NotThrow();
    }

    [Test(Description = "TENCMP2 - CreateEncomendaDTO deve mapear e normalizar campos de texto.")]
    public void CreateEncomendaDTO_Should_MapToEncomendaWithTrim_When_FieldsContainOuterSpaces()
    {
        // ARRANGE
        var dto = new CreateEncomendaDTO
        {
            Cliente_id = 12,
            NumeroEncomendaCliente = " ENC-001 ",
            NumeroProjetoCliente = " PRJ-10 ",
            NomeServicoCliente = " Servico Premium ",
            NomeResponsavelCliente = " Ana "
        };

        // ACT
        var result = _mapper.Map<Encomenda>(dto);

        // ASSERT
        result.Cliente_id.Should().Be(12);
        result.NumeroEncomendaCliente.Should().Be("ENC-001");
        result.NumeroProjetoCliente.Should().Be("PRJ-10");
        result.NomeServicoCliente.Should().Be("Servico Premium");
        result.NomeResponsavelCliente.Should().Be("Ana");
    }

    [Test(Description = "TENCMP3 - UpdateEncomendaDTO deve atualizar apenas campos informados e normalizados.")]
    public void UpdateEncomendaDTO_Should_MapToExistingEncomendaWithoutOverwritingNulls()
    {
        // ARRANGE
        var dto = new UpdateEncomendaDTO
        {
            NumeroEncomendaCliente = " ENC-200 ",
            NumeroProjetoCliente = null,
            NomeServicoCliente = "  Novo Servico  ",
            NomeResponsavelCliente = null
        };

        var destination = new Encomenda
        {
            Encomenda_id = 99,
            NumeroEncomendaCliente = "ENC-100",
            NumeroProjetoCliente = "PRJ-OLD",
            NomeServicoCliente = "Servico Antigo",
            NomeResponsavelCliente = "Maria",
            Estado = EstadoEncomenda.CONFIRMADA,
            Cliente_id = 10
        };

        // ACT
        _mapper.Map(dto, destination);

        // ASSERT
        destination.Encomenda_id.Should().Be(99);
        destination.NumeroEncomendaCliente.Should().Be("ENC-200");
        destination.NumeroProjetoCliente.Should().Be("PRJ-OLD");
        destination.NomeServicoCliente.Should().Be("Novo Servico");
        destination.NomeResponsavelCliente.Should().Be("Maria");
    }

    [Test(Description = "TENCMP4 - Encomenda para ResponseEncomendaDTO deve preservar tipos e mapear NomeCliente.")]
    public void Encomenda_Should_MapToResponseEncomendaDTO_When_DataIsValid()
    {
        // ARRANGE
        var source = new Encomenda
        {
            Encomenda_id = 11,
            NumeroEncomendaCliente = "ENC-011",
            NumeroProjetoCliente = "PRJ-011",
            NomeServicoCliente = "Servico X",
            NomeResponsavelCliente = "Joao",
            DataRegisto = new DateTime(2026, 4, 21, 9, 30, 0, DateTimeKind.Utc),
            Estado = EstadoEncomenda.EM_PRODUCAO,
            Cliente_id = 5,
            Cliente = new Cliente
            {
                Cliente_id = 5,
                Nome = "Cliente XPTO",
                NIF = "123456789",
                Sigla = "CXP"
            }
        };

        // ACT
        var result = _mapper.Map<ResponseEncomendaDTO>(source);

        // ASSERT
        result.Encomenda_id.Should().Be(11);
        result.NumeroEncomendaCliente.Should().Be("ENC-011");
        result.NumeroProjetoCliente.Should().Be("PRJ-011");
        result.NomeServicoCliente.Should().Be("Servico X");
        result.NomeResponsavelCliente.Should().Be("Joao");
        result.DataRegisto.Should().Be(new DateTime(2026, 4, 21, 9, 30, 0, DateTimeKind.Utc));
        result.Estado.Should().Be(EstadoEncomenda.EM_PRODUCAO);
        result.Cliente_id.Should().Be(5);
        result.NomeCliente.Should().Be("Cliente XPTO");
    }

    [Test(Description = "TENCMP5 - UpdateEstadoEncomendaDTO deve atualizar apenas o estado na entidade de destino.")]
    public void UpdateEstadoEncomendaDTO_Should_MapOnlyEstado_When_MappingToExistingEncomenda()
    {
        // ARRANGE
        var dto = new UpdateEstadoEncomendaDTO
        {
            Estado = EstadoEncomenda.EM_PRODUCAO
        };

        var destination = new Encomenda
        {
            Encomenda_id = 21,
            NumeroEncomendaCliente = "ENC-021",
            NumeroProjetoCliente = "PRJ-021",
            NomeServicoCliente = "Servico Original",
            NomeResponsavelCliente = "Ana",
            DataRegisto = new DateTime(2026, 4, 1, 10, 0, 0, DateTimeKind.Utc),
            Estado = EstadoEncomenda.CONFIRMADA,
            Cliente_id = 7
        };

        // ACT
        _mapper.Map(dto, destination);

        // ASSERT
        destination.Estado.Should().Be(EstadoEncomenda.EM_PRODUCAO);
        destination.Encomenda_id.Should().Be(21);
        destination.NumeroEncomendaCliente.Should().Be("ENC-021");
        destination.NumeroProjetoCliente.Should().Be("PRJ-021");
        destination.NomeServicoCliente.Should().Be("Servico Original");
        destination.NomeResponsavelCliente.Should().Be("Ana");
        destination.Cliente_id.Should().Be(7);
        destination.DataRegisto.Should().Be(new DateTime(2026, 4, 1, 10, 0, 0, DateTimeKind.Utc));
    }
}
