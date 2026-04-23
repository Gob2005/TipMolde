using AutoMapper;
using FluentAssertions;
using Moq;
using TipMolde.Application.DTOs.ClienteDTO;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Application.Mappings;
using TipMolde.Application.Service;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
[Category("Unit")]
public class ClienteServiceTests
{
    private Mock<IClienteRepository> _clienteRepository = null!;
    private ClienteService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        // ARRANGE
        _clienteRepository = new Mock<IClienteRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ClienteProfile>();
            cfg.AddProfile<EncomendaProfile>();
        });

        var mapper = mapperConfig.CreateMapper();
        _sut = new ClienteService(_clienteRepository.Object, mapper);
    }

    private static Cliente BuildCliente(
        int id = 1,
        string nome = "Cliente A",
        string nif = "123456789",
        string sigla = "CLA") => new()
        {
            Cliente_id = id,
            Nome = nome,
            NIF = nif,
            Sigla = sigla,
            Pais = "PT",
            Email = "cliente@a.pt",
            Telefone = "910000000"
        };

    private static CreateClienteDTO BuildCreateDto(
        string nome = "Cliente A",
        string nif = "123456789",
        string sigla = "CLA") => new()
        {
            Nome = nome,
            NIF = nif,
            Sigla = sigla,
            Pais = "PT",
            Email = "cliente@a.pt",
            Telefone = "910000000"
        };

    private static UpdateClienteDTO BuildUpdateDto(
        string? nome = "Cliente A",
        string? nif = "123456789",
        string? sigla = "CLA") => new()
        {
            Nome = nome,
            NIF = nif,
            Sigla = sigla,
            Pais = "PT",
            Email = "cliente@a.pt",
            Telefone = "910000000"
        };

    [Test(Description = "T1CLI - Create deve normalizar campos e criar cliente quando dados sao validos.")]
    public async Task CreateAsync_Should_TrimAndCreateCliente_When_DataIsValid()
    {
        // ARRANGE
        var dto = BuildCreateDto(nome: "  Cliente A  ", nif: " 123456789 ", sigla: " cla ");
        dto.Pais = "  PT  ";
        dto.Email = "  cliente@a.pt  ";
        dto.Telefone = " 910000000 ";

        _clienteRepository.Setup(r => r.GetByNifAsync("123456789")).ReturnsAsync((Cliente?)null);
        _clienteRepository.Setup(r => r.GetBySiglaAsync("cla")).ReturnsAsync((Cliente?)null);

        // ACT
        var result = await _sut.CreateAsync(dto);

        // ASSERT
        result.Nome.Should().Be("Cliente A");
        result.NIF.Should().Be("123456789");
        result.Sigla.Should().Be("cla");

        _clienteRepository.Verify(r => r.AddAsync(It.Is<Cliente>(c =>
            c.Nome == "Cliente A" &&
            c.NIF == "123456789" &&
            c.Sigla == "cla" &&
            c.Pais == "PT" &&
            c.Email == "cliente@a.pt" &&
            c.Telefone == "910000000")), Times.Once);
    }

    [Test(Description = "T2CLI - Create deve falhar quando NIF ja existe.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_NifAlreadyExists()
    {
        // ARRANGE
        var dto = BuildCreateDto();
        _clienteRepository.Setup(r => r.GetByNifAsync(dto.NIF)).ReturnsAsync(BuildCliente(id: 7));

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test(Description = "T3CLI - Create deve falhar quando sigla ja existe.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_SiglaAlreadyExists()
    {
        // ARRANGE
        var dto = BuildCreateDto();
        _clienteRepository.Setup(r => r.GetByNifAsync(dto.NIF)).ReturnsAsync((Cliente?)null);
        _clienteRepository.Setup(r => r.GetBySiglaAsync(dto.Sigla)).ReturnsAsync(BuildCliente(id: 7));

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [TestCase("", "123456789", "SIG", Description = "T4CLI-A - Create deve falhar quando nome obrigatorio esta em falta.")]
    [TestCase("Cliente", "", "SIG", Description = "T4CLI-B - Create deve falhar quando NIF obrigatorio esta em falta.")]
    [TestCase("Cliente", "123456789", "", Description = "T4CLI-C - Create deve falhar quando sigla obrigatoria esta em falta.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_RequiredFieldIsMissing(string nome, string nif, string sigla)
    {
        // ARRANGE
        var dto = BuildCreateDto(nome: nome, nif: nif, sigla: sigla);

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test(Description = "T5CLI - Update deve falhar quando cliente nao existe.")]
    public async Task UpdateAsync_Should_ThrowKeyNotFoundException_When_ClienteDoesNotExist()
    {
        // ARRANGE
        _clienteRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cliente?)null);
        var dto = BuildUpdateDto();

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(99, dto);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "T6CLI - Update deve persistir dados normalizados quando cliente existe.")]
    public async Task UpdateAsync_Should_UpdateExistingCliente_When_DataIsValid()
    {
        // ARRANGE
        var existing = BuildCliente(id: 1, nome: "Old Name", nif: "123456789", sigla: "OLD");
        _clienteRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _clienteRepository.Setup(r => r.GetByNifAsync("987654321")).ReturnsAsync((Cliente?)null);
        _clienteRepository.Setup(r => r.GetBySiglaAsync("NEW")).ReturnsAsync((Cliente?)null);

        var dto = BuildUpdateDto(nome: "  New Name  ", nif: "987654321", sigla: "NEW");
        dto.Pais = "  Portugal  ";
        dto.Email = "  novo@tipmolde.pt  ";
        dto.Telefone = " 919999999 ";

        // ACT
        await _sut.UpdateAsync(1, dto);

        // ASSERT
        _clienteRepository.Verify(r => r.UpdateAsync(It.Is<Cliente>(c =>
            c.Cliente_id == 1 &&
            c.Nome == "New Name" &&
            c.NIF == "987654321" &&
            c.Sigla == "NEW" &&
            c.Pais == "Portugal" &&
            c.Email == "novo@tipmolde.pt" &&
            c.Telefone == "919999999")), Times.Once);
    }

    [Test(Description = "T7CLI - Delete deve falhar quando cliente nao existe.")]
    public async Task DeleteAsync_Should_ThrowKeyNotFoundException_When_ClienteDoesNotExist()
    {
        // ARRANGE
        _clienteRepository.Setup(r => r.GetByIdAsync(50)).ReturnsAsync((Cliente?)null);

        // ACT
        Func<Task> act = () => _sut.DeleteAsync(50);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "T8CLI - Delete deve remover cliente quando registo existe.")]
    public async Task DeleteAsync_Should_DeleteCliente_When_ClienteExists()
    {
        // ARRANGE
        _clienteRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildCliente());

        // ACT
        await _sut.DeleteAsync(1);

        // ASSERT
        _clienteRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Test(Description = "T9CLI - Search por nome deve devolver vazio quando termo e branco.")]
    public async Task SearchByNameAsync_Should_ReturnEmpty_When_SearchTermIsBlank()
    {
        // ARRANGE

        // ACT
        var result = await _sut.SearchByNameAsync("   ");

        // ASSERT
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        _clienteRepository.Verify(r => r.SearchByNameAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Test(Description = "T10CLI - Search por sigla deve devolver vazio quando termo e branco.")]
    public async Task SearchBySiglaAsync_Should_ReturnEmpty_When_SearchTermIsBlank()
    {
        // ARRANGE

        // ACT
        var result = await _sut.SearchBySiglaAsync(string.Empty);

        // ASSERT
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        _clienteRepository.Verify(r => r.SearchBySiglaAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Test(Description = "T11CLI - Search por nome deve mapear clientes para DTO paginado quando ha resultados.")]
    public async Task SearchByNameAsync_Should_MapPagedResult_When_RepositoryReturnsItems()
    {
        // ARRANGE
        var clientes = new[] { BuildCliente(id: 10, nome: " Cliente X ", nif: "111111111", sigla: " CX ") };
        var paged = new PagedResult<Cliente>(clientes, 1, 2, 5);

        _clienteRepository
            .Setup(r => r.SearchByNameAsync("Cliente", 2, 5))
            .ReturnsAsync(paged);

        // ACT
        var result = await _sut.SearchByNameAsync(" Cliente ", 2, 5);

        // ASSERT
        result.TotalCount.Should().Be(1);
        result.CurrentPage.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.Items.Should().ContainSingle();
        result.Items.Single().Cliente_id.Should().Be(10);
        result.Items.Single().Nome.Should().Be("Cliente X");
        result.Items.Single().Sigla.Should().Be("CX");
    }
}

