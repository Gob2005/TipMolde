using AutoMapper;
using FluentAssertions;
using TipMolde.Application.DTOs.ClienteDTO;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Mapping
{
    [TestFixture]
    public class ClienteProfileTestsTests
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
        public void shouldHaveValidAutoMapperConfiguration()
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

        [Test]
        public void shouldMapClienteToResponseClienteWithEncomendasDto()
        {
            var cliente = new Cliente
            {
                Cliente_id = 42,
                Nome = "  Cliente A  ",
                NIF = " 123456789 ",
                Sigla = "  CA ",
                Pais = " PT ",
                Email = " cliente@tipmolde.pt ",
                Telefone = " 910000000 ",
                Encomendas = new List<Encomenda>
        {
            new()
            {
                Encomenda_id = 1001,
                NumeroEncomendaCliente = "ENC-1001",
                NumeroProjetoCliente = "PRJ-77",
                NomeServicoCliente = "Molde X",
                NomeResponsavelCliente = "Maria",
                DataRegisto = new DateTime(2026, 4, 1),
                Estado = EstadoEncomenda.CONFIRMADA,
                Cliente_id = 42,
                Cliente = new Cliente
                {
                    Cliente_id = 42,
                    Nome = "Cliente A",
                    NIF = "123456789",
                    Sigla = "CA"
                }
            }
        }
            };

            var result = _mapper.Map<ResponseClienteWithEncomendasDTO>(cliente);

            result.ClienteId.Should().Be(42);
            result.Nome.Should().Be("Cliente A");
            result.NIF.Should().Be("123456789");
            result.Sigla.Should().Be("CA");
            result.Pais.Should().Be("PT");
            result.Email.Should().Be("cliente@tipmolde.pt");
            result.Telefone.Should().Be("910000000");

            result.Encomendas.Should().NotBeNull();
            result.Encomendas!.Should().HaveCount(1);
            result.Encomendas.First().Encomenda_id.Should().Be(1001);
            result.Encomendas.First().NumeroEncomendaCliente.Should().Be("ENC-1001");
            result.Encomendas.First().NomeCliente.Should().Be("Cliente A");
        }
    }
}
