using Moq;
using TipMolde.Core.Interface.Comercio.ICliente;
using TipMolde.Core.Models.Comercio;
using TipMolde.Infrastructure.Service;

namespace TipMolde.Tests.Unitario
{
    public class ClienteServiceTests
    {
        private readonly Mock<IClienteRepository> _clienteRepository = new();
        private readonly ClienteService _sut;

        public ClienteServiceTests()
        {
            _sut = new ClienteService(_clienteRepository.Object);
        }

        private static Cliente ClienteFake(
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

        [Fact]
        public async Task CreateClienteAsync_ValidData_AddsCliente()
        {
            var cliente = ClienteFake(nome: "  Cliente A  ", nif: " 123456789 ", sigla: " cla ");

            _clienteRepository.Setup(r => r.GetByNifAsync("123456789")).ReturnsAsync((Cliente?)null);
            _clienteRepository.Setup(r => r.GetBySiglaAsync("cla")).ReturnsAsync((Cliente?)null);
            _clienteRepository.Setup(r => r.AddAsync(It.IsAny<Cliente>())).Returns(Task.CompletedTask);

            var result = await _sut.CreateClienteAsync(cliente);

            Assert.Equal("Cliente A", result.Nome);
            Assert.Equal("123456789", result.NIF);
            Assert.Equal("cla", result.Sigla);
            _clienteRepository.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact]
        public async Task CreateClienteAsync_DuplicateNif_Throws()
        {
            var cliente = ClienteFake();
            _clienteRepository.Setup(r => r.GetByNifAsync(cliente.NIF)).ReturnsAsync(ClienteFake(id: 7));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateClienteAsync(cliente));
        }

        [Fact]
        public async Task CreateClienteAsync_DuplicateSigla_Throws()
        {
            var cliente = ClienteFake();
            _clienteRepository.Setup(r => r.GetByNifAsync(cliente.NIF)).ReturnsAsync((Cliente?)null);
            _clienteRepository.Setup(r => r.GetBySiglaAsync(cliente.Sigla)).ReturnsAsync(ClienteFake(id: 7));

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateClienteAsync(cliente));
        }

        [Theory]
        [InlineData("", "123456789", "SIG")]
        [InlineData("Cliente", "", "SIG")]
        [InlineData("Cliente", "123456789", "")]
        public async Task CreateClienteAsync_MissingRequiredField_Throws(string nome, string nif, string sigla)
        {
            var cliente = ClienteFake(nome: nome, nif: nif, sigla: sigla);
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateClienteAsync(cliente));
        }

        [Fact]
        public async Task UpdateClienteAsync_NotFound_Throws()
        {
            _clienteRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Cliente?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateClienteAsync(ClienteFake(id: 99)));
        }

        [Fact]
        public async Task UpdateClienteAsync_ValidData_UpdatesExisting()
        {
            var existing = ClienteFake(id: 1, nome: "Old Name", nif: "123456789", sigla: "OLD");

            _clienteRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _clienteRepository.Setup(r => r.GetByNifAsync("987654321")).ReturnsAsync((Cliente?)null);
            _clienteRepository.Setup(r => r.GetBySiglaAsync("NEW")).ReturnsAsync((Cliente?)null);
            _clienteRepository.Setup(r => r.UpdateAsync(It.IsAny<Cliente>())).Returns(Task.CompletedTask);

            var update = ClienteFake(id: 1, nome: "  New Name  ", nif: "987654321", sigla: "NEW");
            update.Pais = "  Portugal  ";

            await _sut.UpdateClienteAsync(update);

            _clienteRepository.Verify(r => r.UpdateAsync(It.Is<Cliente>(c =>
                c.Cliente_id == 1 &&
                c.Nome == "New Name" &&
                c.NIF == "987654321" &&
                c.Sigla == "NEW" &&
                c.Pais == "Portugal")), Times.Once);
        }

        [Fact]
        public async Task DeleteClienteAsync_NotFound_Throws()
        {
            _clienteRepository.Setup(r => r.GetByIdAsync(50)).ReturnsAsync((Cliente?)null);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteClienteAsync(50));
        }

        [Fact]
        public async Task DeleteClienteAsync_Found_Deletes()
        {
            _clienteRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ClienteFake());
            _clienteRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            await _sut.DeleteClienteAsync(1);

            _clienteRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task SearchByNameAsync_EmptyTerm_ReturnsEmptyWithoutRepo()
        {
            var result = await _sut.SearchByNameAsync("   ");
            Assert.Empty(result);
            _clienteRepository.Verify(r => r.SearchByNameAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SearchBySiglaAsync_EmptyTerm_ReturnsEmptyWithoutRepo()
        {
            var result = await _sut.SearchBySiglaAsync("");
            Assert.Empty(result);
            _clienteRepository.Verify(r => r.SearchBySiglaAsync(It.IsAny<string>()), Times.Never);
        }
    }
}
