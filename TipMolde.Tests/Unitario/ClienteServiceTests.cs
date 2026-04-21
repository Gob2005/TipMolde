using FluentAssertions;
using Moq;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Application.Service;

namespace TipMolde.Tests.Unitario;

[TestFixture]
public class ClienteServiceTests
{
    private Mock<IClienteRepository> _clienteRepository = null!;
    private ClienteService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _clienteRepository = new Mock<IClienteRepository>();
        _sut = new ClienteService(_clienteRepository.Object);
    }

    /// <summary>
    /// Helper de teste para criar entidades Cliente com valores predefinidos.
    /// </summary>
    /// <param name="id">Identificador do cliente.</param>
    /// <param name="nome">Nome do cliente.</param>
    /// <param name="nif">NIF do cliente.</param>
    /// <param name="sigla">Sigla do cliente.</param>
    /// <returns>Instancia de Cliente para composicao de cenarios de teste.</returns>
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

    [Test(Description = "T1CLI - Create deve normalizar campos e criar cliente quando dados sao validos.")]
    public async Task CreateAsync_Should_TrimAndCreateCliente_When_DataIsValid()
    {
        // ARRANGE
        var cliente = BuildCliente(nome: "  Cliente A  ", nif: " 123456789 ", sigla: " cla ");
        _clienteRepository.Setup(r => r.GetByNifAsync("123456789")).ReturnsAsync((Cliente?)null);
        _clienteRepository.Setup(r => r.GetBySiglaAsync("cla")).ReturnsAsync((Cliente?)null);

        // ACT
        var result = await _sut.CreateAsync(cliente);

        // ASSERT
        result.Nome.Should().Be("Cliente A");
        result.NIF.Should().Be("123456789");
        result.Sigla.Should().Be("cla");
        _clienteRepository.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Test(Description = "T2CLI - Create deve falhar quando NIF ja existe.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_NifAlreadyExists()
    {
        // ARRANGE
        var cliente = BuildCliente();
        _clienteRepository.Setup(r => r.GetByNifAsync(cliente.NIF)).ReturnsAsync(BuildCliente(id: 7));

        // ACT
        Func<Task> act = () => _sut.CreateAsync(cliente);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test(Description = "T3CLI - Create deve falhar quando sigla ja existe.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_SiglaAlreadyExists()
    {
        // ARRANGE
        var cliente = BuildCliente();
        _clienteRepository.Setup(r => r.GetByNifAsync(cliente.NIF)).ReturnsAsync((Cliente?)null);
        _clienteRepository.Setup(r => r.GetBySiglaAsync(cliente.Sigla)).ReturnsAsync(BuildCliente(id: 7));

        // ACT
        Func<Task> act = () => _sut.CreateAsync(cliente);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [TestCase("", "123456789", "SIG", Description = "T4CLI-A - Create deve falhar quando nome obrigatorio esta em falta.")]
    [TestCase("Cliente", "", "SIG", Description = "T4CLI-B - Create deve falhar quando NIF obrigatorio esta em falta.")]
    [TestCase("Cliente", "123456789", "", Description = "T4CLI-C - Create deve falhar quando sigla obrigatoria esta em falta.")]
    public async Task CreateAsync_Should_ThrowArgumentException_When_RequiredFieldIsMissing(string nome, string nif, string sigla)
    {
        // ARRANGE
        var cliente = BuildCliente(nome: nome, nif: nif, sigla: sigla);

        // ACT
        Func<Task> act = () => _sut.CreateAsync(cliente);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test(Description = "T5CLI - Update deve falhar quando cliente nao existe.")]
    public async Task UpdateAsync_Should_ThrowKeyNotFoundException_When_ClienteDoesNotExist()
    {
        // ARRANGE
        _clienteRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cliente?)null);

        // ACT
        Func<Task> act = () => _sut.UpdateAsync(BuildCliente(id: 99));

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

        var update = BuildCliente(id: 1, nome: "  New Name  ", nif: "987654321", sigla: "NEW");
        update.Pais = "  Portugal  ";

        // ACT
        await _sut.UpdateAsync(update);

        // ASSERT
        _clienteRepository.Verify(r => r.UpdateAsync(It.Is<Cliente>(c =>
            c.Cliente_id == 1 &&
            c.Nome == "New Name" &&
            c.NIF == "987654321" &&
            c.Sigla == "NEW" &&
            c.Pais == "Portugal")), Times.Once);
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
        result.Should().BeEmpty();
        _clienteRepository.Verify(r => r.SearchByNameAsync(It.IsAny<string>()), Times.Never);
    }

    [Test(Description = "T10CLI - Search por sigla deve devolver vazio quando termo e branco.")]
    public async Task SearchBySiglaAsync_Should_ReturnEmpty_When_SearchTermIsBlank()
    {
        // ARRANGE

        // ACT
        var result = await _sut.SearchBySiglaAsync(string.Empty);

        // ASSERT
        result.Should().BeEmpty();
        _clienteRepository.Verify(r => r.SearchBySiglaAsync(It.IsAny<string>()), Times.Never);
    }
}

