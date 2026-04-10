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

    [Test]
    public async Task shouldTrimAndCreateClienteWhenDataIsValid()
    {
        // Arrange
        var cliente = BuildCliente(nome: "  Cliente A  ", nif: " 123456789 ", sigla: " cla ");
        _clienteRepository.Setup(r => r.GetByNifAsync("123456789")).ReturnsAsync((Cliente?)null);
        _clienteRepository.Setup(r => r.GetBySiglaAsync("cla")).ReturnsAsync((Cliente?)null);

        // Act
        var result = await _sut.CreateAsync(cliente);

        // Assert
        result.Nome.Should().Be("Cliente A");
        result.NIF.Should().Be("123456789");
        result.Sigla.Should().Be("cla");
        _clienteRepository.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Once);
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenNifAlreadyExists()
    {
        // Arrange
        var cliente = BuildCliente();
        _clienteRepository.Setup(r => r.GetByNifAsync(cliente.NIF)).ReturnsAsync(BuildCliente(id: 7));

        // Act
        Func<Task> act = () => _sut.CreateAsync(cliente);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenSiglaAlreadyExists()
    {
        // Arrange
        var cliente = BuildCliente();
        _clienteRepository.Setup(r => r.GetByNifAsync(cliente.NIF)).ReturnsAsync((Cliente?)null);
        _clienteRepository.Setup(r => r.GetBySiglaAsync(cliente.Sigla)).ReturnsAsync(BuildCliente(id: 7));

        // Act
        Func<Task> act = () => _sut.CreateAsync(cliente);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [TestCase("", "123456789", "SIG")]
    [TestCase("Cliente", "", "SIG")]
    [TestCase("Cliente", "123456789", "")]
    public async Task shouldThrowArgumentExceptionWhenRequiredFieldIsMissing(string nome, string nif, string sigla)
    {
        // Arrange
        var cliente = BuildCliente(nome: nome, nif: nif, sigla: sigla);

        // Act
        Func<Task> act = () => _sut.CreateAsync(cliente);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenUpdatingUnknownCliente()
    {
        // Arrange
        _clienteRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cliente?)null);

        // Act
        Func<Task> act = () => _sut.UpdateAsync(BuildCliente(id: 99));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldUpdateExistingClienteWhenDataIsValid()
    {
        // Arrange
        var existing = BuildCliente(id: 1, nome: "Old Name", nif: "123456789", sigla: "OLD");
        _clienteRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _clienteRepository.Setup(r => r.GetByNifAsync("987654321")).ReturnsAsync((Cliente?)null);
        _clienteRepository.Setup(r => r.GetBySiglaAsync("NEW")).ReturnsAsync((Cliente?)null);

        var update = BuildCliente(id: 1, nome: "  New Name  ", nif: "987654321", sigla: "NEW");
        update.Pais = "  Portugal  ";

        // Act
        await _sut.UpdateAsync(update);

        // Assert
        _clienteRepository.Verify(r => r.UpdateAsync(It.Is<Cliente>(c =>
            c.Cliente_id == 1 &&
            c.Nome == "New Name" &&
            c.NIF == "987654321" &&
            c.Sigla == "NEW" &&
            c.Pais == "Portugal")), Times.Once);
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenDeletingUnknownCliente()
    {
        // Arrange
        _clienteRepository.Setup(r => r.GetByIdAsync(50)).ReturnsAsync((Cliente?)null);

        // Act
        Func<Task> act = () => _sut.DeleteAsync(50);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldDeleteClienteWhenClienteExists()
    {
        // Arrange
        _clienteRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildCliente());

        // Act
        await _sut.DeleteAsync(1);

        // Assert
        _clienteRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Test]
    public async Task shouldReturnEmptyWhenSearchByNameTermIsBlank()
    {
        // Arrange
        // no setup needed

        // Act
        var result = await _sut.SearchByNameAsync("   ");

        // Assert
        result.Should().BeEmpty();
        _clienteRepository.Verify(r => r.SearchByNameAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task shouldReturnEmptyWhenSearchBySiglaTermIsBlank()
    {
        // Arrange
        // no setup needed

        // Act
        var result = await _sut.SearchBySiglaAsync(string.Empty);

        // Assert
        result.Should().BeEmpty();
        _clienteRepository.Verify(r => r.SearchBySiglaAsync(It.IsAny<string>()), Times.Never);
    }
}

