using AutoMapper;
using FluentAssertions;
using TipMolde.Application.DTOs.ClienteDTO;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Tests.Unitario;

[TestFixture]
public class MappingProfilesTests
{
    private IMapper _mapper = null!;

    [SetUp]
    public void SetUp()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ClienteProfile>();
        });

        _mapper = config.CreateMapper();
    }

    [Test]
    public void shouldHaveValidClienteAutoMapperConfiguration()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ClienteProfile>();
        });

        config.AssertConfigurationIsValid();
    }

    [Test]
    public void shouldMapCreateClienteDtoToClienteWithTrimmedFields()
    {
        var source = new CreateClienteDTO
        {
            Nome = "  Cliente Teste  ",
            NIF = " 123456789 ",
            Sigla = "  CT  ",
            Pais = "PT",
            Email = "cliente@tipmolde.pt",
            Telefone = "912345678"
        };

        var result = _mapper.Map<Cliente>(source);

        result.Nome.Should().Be("Cliente Teste");
        result.NIF.Should().Be("123456789");
        result.Sigla.Should().Be("CT");
        result.Pais.Should().Be("PT");
        result.Email.Should().Be("cliente@tipmolde.pt");
        result.Telefone.Should().Be("912345678");
    }

    [Test]
    public void shouldMapUpdateClienteDtoToExistingClienteWithoutOverwritingNulls()
    {
        var source = new UpdateClienteDTO
        {
            Nome = "  Novo Nome  ",
            NIF = null,
            Sigla = "  NN  ",
            Pais = null,
            Email = "novo@tipmolde.pt",
            Telefone = null
        };

        var destination = new Cliente
        {
            Cliente_id = 10,
            Nome = "Nome Antigo",
            NIF = "123456789",
            Sigla = "OLD",
            Pais = "Portugal",
            Email = "antigo@tipmolde.pt",
            Telefone = "910000000"
        };

        _mapper.Map(source, destination);

        destination.Cliente_id.Should().Be(10);
        destination.Nome.Should().Be("  Novo Nome  ");
        destination.NIF.Should().Be("123456789");
        destination.Sigla.Should().Be("  NN  ");
        destination.Pais.Should().Be("Portugal");
        destination.Email.Should().Be("novo@tipmolde.pt");
        destination.Telefone.Should().Be("910000000");
    }

    [Test]
    public void shouldMapClienteToResponseClienteDto()
    {
        var source = new Cliente
        {
            Cliente_id = 42,
            Nome = "Cliente A",
            NIF = "123456789",
            Sigla = "CA",
            Pais = "PT",
            Email = "a@tipmolde.pt",
            Telefone = "911111111"
        };

        var result = _mapper.Map<ResponseClienteDTO>(source);

        result.ClienteId.Should().Be(42);
        result.Nome.Should().Be("Cliente A");
        result.NIF.Should().Be("123456789");
        result.Sigla.Should().Be("CA");
        result.Pais.Should().Be("PT");
        result.Email.Should().Be("a@tipmolde.pt");
        result.Telefone.Should().Be("911111111");
    }
}
