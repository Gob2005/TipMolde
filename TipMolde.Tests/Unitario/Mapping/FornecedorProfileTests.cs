using AutoMapper;
using FluentAssertions;
using TipMolde.Application.Dtos.FornecedorDto;
using TipMolde.Application.Mappings;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Tests.Unitario.Mapping
{
    [TestFixture]
[Category("Unit")]
    public class FornecedorProfileTests
    {
        private IMapper _mapper = null!;

        [SetUp]
        public void SetUp()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FornecedorProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Test(Description = "T1MAPFOR - Configuracao do AutoMapper para Fornecedor e valida.")]
        public void MappingConfiguration_Should_BeValid_When_ProfileIsLoaded()
        {
            // ARRANGE
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FornecedorProfile>();
            });

            // ACT
            Action act = () => config.AssertConfigurationIsValid();

            // ASSERT
            act.Should().NotThrow();
        }

        [Test(Description = "T2MAPFOR - CreateFornecedorDto aplica trim nos campos de texto.")]
        public void CreateFornecedorDTO_Should_MapWithTrim_When_FieldsContainOuterSpaces()
        {
            // ARRANGE
            var source = new CreateFornecedorDto
            {
                Nome = "  Fornecedor Teste  ",
                NIF = " 123456789 ",
                Morada = "  Rua Principal  ",
                Email = "  fornecedor@tipmolde.pt  ",
                Telefone = "  912345678  "
            };

            // ACT
            var result = _mapper.Map<Fornecedor>(source);

            // ASSERT
            result.Nome.Should().Be("Fornecedor Teste");
            result.NIF.Should().Be("123456789");
            result.Morada.Should().Be("Rua Principal");
            result.Email.Should().Be("fornecedor@tipmolde.pt");
            result.Telefone.Should().Be("912345678");
        }

        [Test(Description = "T3MAPFOR - UpdateFornecedorDto aplica trim e preserva valores quando campos sao nulos.")]
        public void UpdateFornecedorDTO_Should_TrimAndPreserveValues_When_NullAndWhitespaceFieldsAreProvided()
        {
            // ARRANGE
            var source = new UpdateFornecedorDto
            {
                Nome = "  Novo Nome  ",
                NIF = null,
                Morada = "  Nova Morada  ",
                Email = "  novo@tipmolde.pt  ",
                Telefone = null
            };

            var destination = new Fornecedor
            {
                Fornecedor_id = 10,
                Nome = "Nome Antigo",
                NIF = "123456789",
                Morada = "Morada Antiga",
                Email = "antigo@tipmolde.pt",
                Telefone = "910000000"
            };

            // ACT
            _mapper.Map(source, destination);

            // ASSERT
            destination.Fornecedor_id.Should().Be(10);
            destination.Nome.Should().Be("Novo Nome");
            destination.NIF.Should().Be("123456789");
            destination.Morada.Should().Be("Nova Morada");
            destination.Email.Should().Be("novo@tipmolde.pt");
            destination.Telefone.Should().Be("910000000");
        }

        [Test(Description = "T4MAPFOR - UpdateFornecedorDto ignora campos apenas com espacos para nao sobrescrever dados existentes.")]
        public void UpdateFornecedorDTO_Should_IgnoreWhitespaceOnlyValues_When_MappingToExistingEntity()
        {
            // ARRANGE
            var source = new UpdateFornecedorDto
            {
                Nome = "   ",
                NIF = "   ",
                Morada = "   ",
                Email = "   ",
                Telefone = "   "
            };

            var destination = new Fornecedor
            {
                Fornecedor_id = 99,
                Nome = "Nome Atual",
                NIF = "123456789",
                Morada = "Morada Atual",
                Email = "atual@tipmolde.pt",
                Telefone = "919999999"
            };

            // ACT
            _mapper.Map(source, destination);

            // ASSERT
            destination.Fornecedor_id.Should().Be(99);
            destination.Nome.Should().Be("Nome Atual");
            destination.NIF.Should().Be("123456789");
            destination.Morada.Should().Be("Morada Atual");
            destination.Email.Should().Be("atual@tipmolde.pt");
            destination.Telefone.Should().Be("919999999");
        }

        [Test(Description = "T5MAPFOR - Fornecedor para ResponseFornecedorDto devolve campos normalizados com trim.")]
        public void Fornecedor_Should_MapToResponseFornecedorDTOWithTrim_When_SourceContainsOuterSpaces()
        {
            // ARRANGE
            var source = new Fornecedor
            {
                Fornecedor_id = 42,
                Nome = "  Fornecedor A  ",
                NIF = " 123456789 ",
                Morada = " Rua A ",
                Email = " a@tipmolde.pt ",
                Telefone = " 911111111 "
            };

            // ACT
            var result = _mapper.Map<ResponseFornecedorDto>(source);

            // ASSERT
            result.FornecedorId.Should().Be(42);
            result.Nome.Should().Be("Fornecedor A");
            result.NIF.Should().Be("123456789");
            result.Morada.Should().Be("Rua A");
            result.Email.Should().Be("a@tipmolde.pt");
            result.Telefone.Should().Be("911111111");
        }
    }
}
