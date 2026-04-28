using AutoMapper;
using FluentAssertions;
using Moq;
using TipMolde.Application.Dtos.FornecedorDto;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.IFornecedor;
using TipMolde.Application.Mappings;
using TipMolde.Application.Service;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
[Category("Unit")]
public class FornecedorServiceTests
{
    private Mock<IFornecedorRepository> _fornecedorRepository = null!;
    private FornecedorService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        // ARRANGE
        _fornecedorRepository = new Mock<IFornecedorRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FornecedorProfile>();
        });

        var mapper = mapperConfig.CreateMapper();
        _sut = new FornecedorService(_fornecedorRepository.Object, mapper);
    }

    private static Fornecedor BuildFornecedor(
        int id = 1,
        string nome = "Fornecedor A",
        string nif = "123456789") => new()
        {
            Fornecedor_id = id,
            Nome = nome,
            NIF = nif,
            Morada = "Rua A",
            Email = "fornecedor@tipmolde.pt",
            Telefone = "910000000"
        };

    private static CreateFornecedorDto BuildCreateDto(
        string nome = "Fornecedor A",
        string nif = "123456789") => new()
        {
            Nome = nome,
            NIF = nif,
            Morada = "Rua A",
            Email = "fornecedor@tipmolde.pt",
            Telefone = "910000000"
        };

    private static UpdateFornecedorDto BuildUpdateDto(
        string? nome = "Fornecedor A",
        string? nif = "123456789") => new()
        {
            Nome = nome,
            NIF = nif,
            Morada = "Rua A",
            Email = "fornecedor@tipmolde.pt",
            Telefone = "910000000"
        };

    [Test(Description = "T1FORSER - Create deve normalizar campos e criar fornecedor quando dados sao validos.")]
    public async Task CreateAsync_Should_TrimAndCreateFornecedor_When_DataIsValid()
    {
        // ARRANGE
        var dto = BuildCreateDto(nome: "  Fornecedor A  ", nif: " 123456789 ");
        dto.Morada = "  Rua Principal  ";
        dto.Email = "  fornecedor@tipmolde.pt  ";
        dto.Telefone = " 910000000 ";

        _fornecedorRepository
            .Setup(r => r.GetByNifAsync("123456789"))
            .ReturnsAsync((Fornecedor?)null);

        // ACT
        var result = await _sut.CreateAsync(dto);

        // ASSERT
        result.Nome.Should().Be("Fornecedor A");
        result.NIF.Should().Be("123456789");
        result.Morada.Should().Be("Rua Principal");
        result.Email.Should().Be("fornecedor@tipmolde.pt");
        result.Telefone.Should().Be("910000000");

        _fornecedorRepository.Verify(r => r.AddAsync(It.Is<Fornecedor>(f =>
            f.Nome == "Fornecedor A" &&
            f.NIF == "123456789" &&
            f.Morada == "Rua Principal" &&
            f.Email == "fornecedor@tipmolde.pt" &&
            f.Telefone == "910000000")), Times.Once);
    }

    [Test(Description = "T2FORSER - Create deve falhar quando NIF ja existe.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_NifAlreadyExists()
    {
        // ARRANGE
        var dto = BuildCreateDto();
        _fornecedorRepository
            .Setup(r => r.GetByNifAsync(dto.NIF))
            .ReturnsAsync(BuildFornecedor(id: 7));

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [TestCase("", "123456789", Description = "T3FORSER-A - Create deve falhar quando nome obrigatorio esta em falta.")]
    [TestCase("Fornecedor A", "", Description = "T3FORSER-B - Create deve falhar quando NIF obrigatorio esta em falta.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_RequiredFieldIsMissing(string nome, string nif)
    {
        // ARRANGE
        var dto = BuildCreateDto(nome: nome, nif: nif);

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test(Description = "T4FORSER - Update deve falhar quando fornecedor nao existe.")]
    public async Task UpdateAsync_Should_ThrowKeyNotFoundException_When_FornecedorDoesNotExist()
    {
        // ARRANGE
        _fornecedorRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Fornecedor?)null);
        var dto = BuildUpdateDto();

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(99, dto);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "T5FORSER - Update deve persistir dados normalizados quando fornecedor existe.")]
    public async Task UpdateAsync_Should_UpdateExistingFornecedor_When_DataIsValid()
    {
        // ARRANGE
        var existing = BuildFornecedor(id: 1, nome: "Old Name", nif: "123456789");
        _fornecedorRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _fornecedorRepository.Setup(r => r.GetByNifAsync("987654321")).ReturnsAsync((Fornecedor?)null);

        var dto = BuildUpdateDto(nome: "  New Name  ", nif: "987654321");
        dto.Morada = "  Nova Morada  ";
        dto.Email = "  novo@tipmolde.pt  ";
        dto.Telefone = " 919999999 ";

        // ACT
        await _sut.UpdateAsync(1, dto);

        // ASSERT
        _fornecedorRepository.Verify(r => r.UpdateAsync(It.Is<Fornecedor>(f =>
            f.Fornecedor_id == 1 &&
            f.Nome == "New Name" &&
            f.NIF == "987654321" &&
            f.Morada == "Nova Morada" &&
            f.Email == "novo@tipmolde.pt" &&
            f.Telefone == "919999999")), Times.Once);
    }

    [Test(Description = "T6FORSER - Update deve preservar valores existentes quando pedido e parcial.")]
    public async Task UpdateAsync_Should_PreserveExistingValues_When_FieldsAreNullOrWhitespace()
    {
        // ARRANGE
        var existing = BuildFornecedor(id: 1, nome: "Fornecedor Atual", nif: "123456789");
        existing.Morada = "Morada Atual";
        existing.Email = "atual@tipmolde.pt";
        existing.Telefone = "911111111";

        _fornecedorRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var dto = new UpdateFornecedorDto
        {
            Nome = "   ",
            NIF = null,
            Morada = "   ",
            Email = null,
            Telefone = "   "
        };

        // ACT
        await _sut.UpdateAsync(1, dto);

        // ASSERT
        _fornecedorRepository.Verify(r => r.UpdateAsync(It.Is<Fornecedor>(f =>
            f.Fornecedor_id == 1 &&
            f.Nome == "Fornecedor Atual" &&
            f.NIF == "123456789" &&
            f.Morada == "Morada Atual" &&
            f.Email == "atual@tipmolde.pt" &&
            f.Telefone == "911111111")), Times.Once);
    }

    [Test(Description = "T7FORSER - Update deve falhar quando NIF novo ja pertence a outro fornecedor.")]
    public async Task UpdateAsync_Should_ThrowArgumentException_When_NewNifAlreadyExists()
    {
        // ARRANGE
        var existing = BuildFornecedor(id: 1, nif: "123456789");
        _fornecedorRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _fornecedorRepository.Setup(r => r.GetByNifAsync("987654321")).ReturnsAsync(BuildFornecedor(id: 2, nif: "987654321"));

        var dto = BuildUpdateDto(nif: "987654321");

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(1, dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test(Description = "T8FORSER - Delete deve falhar quando fornecedor nao existe.")]
    public async Task DeleteAsync_Should_ThrowKeyNotFoundException_When_FornecedorDoesNotExist()
    {
        // ARRANGE
        _fornecedorRepository.Setup(r => r.GetByIdAsync(50)).ReturnsAsync((Fornecedor?)null);

        // ACT
        Func<Task> act = () => _sut.DeleteAsync(50);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "T9FORSER - Delete deve remover fornecedor quando registo existe.")]
    public async Task DeleteAsync_Should_DeleteFornecedor_When_FornecedorExists()
    {
        // ARRANGE
        _fornecedorRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFornecedor());

        // ACT
        await _sut.DeleteAsync(1);

        // ASSERT
        _fornecedorRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Test(Description = "T10FORSER - Search por nome deve devolver vazio quando termo e branco.")]
    public async Task SearchByNameAsync_Should_ReturnEmpty_When_SearchTermIsBlank()
    {
        // ARRANGE

        // ACT
        var result = await _sut.SearchByNameAsync("   ", 1, 10);

        // ASSERT
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        _fornecedorRepository.Verify(r => r.SearchByNameAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Test(Description = "T11FORSER - Search por nome deve mapear fornecedores para DTO paginado quando ha resultados.")]
    public async Task SearchByNameAsync_Should_MapPagedResult_When_RepositoryReturnsItems()
    {
        // ARRANGE
        var fornecedores = new[]
        {
            BuildFornecedor(id: 10, nome: " Fornecedor X ", nif: "111111111")
        };

        var paged = new PagedResult<Fornecedor>(fornecedores, 1, 2, 10);

        _fornecedorRepository
            .Setup(r => r.SearchByNameAsync("Fornecedor", 2, 10))
            .ReturnsAsync(paged);

        // ACT
        var result = await _sut.SearchByNameAsync(" Fornecedor ", 2, 5);

        // ASSERT
        result.TotalCount.Should().Be(1);
        result.CurrentPage.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.Items.Should().ContainSingle();
        result.Items.Single().FornecedorId.Should().Be(10);
        result.Items.Single().Nome.Should().Be("Fornecedor X");
        result.Items.Single().NIF.Should().Be("111111111");
    }
}
