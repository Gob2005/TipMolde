using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TipMolde.Application.Dtos.ClienteDto;
using TipMolde.Application.Interface;

namespace TipMolde.Tests.Integracao.Controller
{
    [TestFixture]
    [Category("Integration")]
    public sealed class ClienteControllerTests : ControllerHttpTestBase
    {
        [Test(Description = "TCLIAPI1 - GET /api/clientes devolve ProblemDetails quando paginacao e invalida.")]
        public async Task GetAllClientes_Should_ReturnProblemDetails_When_PaginationIsInvalid()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/clientes?page=0&pageSize=10");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
            Factory.ClienteService.Verify(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test(Description = "TCLIAPI2 - GET /api/clientes/{id} devolve ProblemDetails quando cliente nao existe.")]
        public async Task GetClienteById_Should_ReturnProblemDetails_When_ClienteDoesNotExist()
        {
            // ARRANGE
            Factory.ClienteService
                .Setup(s => s.GetByIdAsync(44))
                .ReturnsAsync((ResponseClienteDto?)null);

            // ACT
            var response = await Client.GetAsync("/api/clientes/44");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.NotFound, "Recurso nao encontrado");
        }

        [Test(Description = "TCLIAPI3 - POST /api/clientes devolve 201 e JSON do cliente criado quando request e valida.")]
        public async Task CreateCliente_Should_ReturnCreatedJson_When_RequestIsValid()
        {
            // ARRANGE
            var created = new ResponseClienteDto
            {
                Cliente_id = 12,
                Nome = "Cliente A",
                NIF = "123456789",
                Sigla = "CLA"
            };

            Factory.ClienteService
                .Setup(s => s.CreateAsync(It.IsAny<CreateClienteDto>()))
                .ReturnsAsync(created);

            var payload = new
            {
                nome = "Cliente A",
                nif = "123456789",
                sigla = "CLA",
                email = "cliente@tipmolde.pt"
            };

            // ACT
            var response = await Client.PostAsJsonAsync("/api/clientes", payload);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var body = await response.Content.ReadFromJsonAsync<ResponseClienteDto>();
            body.Should().BeEquivalentTo(created);
        }

        [Test(Description = "TCLIAPI4 - PUT /api/clientes/{id} devolve 204 quando cliente existe.")]
        public async Task UpdateCliente_Should_ReturnNoContent_When_ClienteExists()
        {
            // ARRANGE
            Factory.ClienteService
                .Setup(s => s.GetByIdAsync(12))
                .ReturnsAsync(new ResponseClienteDto { Cliente_id = 12, Nome = "Cliente A" });
            Factory.ClienteService
                .Setup(s => s.UpdateAsync(12, It.IsAny<UpdateClienteDto>()))
                .Returns(Task.CompletedTask);

            // ACT
            var response = await Client.PutAsJsonAsync("/api/clientes/12", new { nome = "Cliente Atualizado" });

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            Factory.ClienteService.Verify(s => s.UpdateAsync(12, It.IsAny<UpdateClienteDto>()), Times.Once);
        }

        [Test(Description = "TCLIAPI5 - GET /api/clientes/{id}/encomendas devolve 404 quando cliente nao existe.")]
        public async Task GetClienteWithEncomendas_Should_ReturnProblemDetails_When_ClienteDoesNotExist()
        {
            // ARRANGE
            Factory.ClienteService
                .Setup(s => s.GetClienteWithEncomendasAsync(44))
                .ReturnsAsync((ResponseClienteWithEncomendasDto?)null);

            // ACT
            var response = await Client.GetAsync("/api/clientes/44/encomendas");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.NotFound, "Recurso nao encontrado");
        }

        [Test(Description = "TCLIAPI7 - GET /api/clientes/search/by-sigla devolve 200 e chama service quando query e valida.")]
        public async Task SearchBySigla_Should_ReturnOkJson_When_QueryIsValid()
        {
            // ARRANGE
            var result = new PagedResult<ResponseClienteDto>(
                new[] { new ResponseClienteDto { Cliente_id = 7, Nome = "Cliente Alfa", Sigla = "ALF" } },
                1,
                2,
                3);

            Factory.ClienteService
                .Setup(s => s.SearchBySiglaAsync("AL", 2, 3))
                .ReturnsAsync(result);

            // ACT
            var response = await Client.GetAsync("/api/clientes/search/by-sigla?searchTerm=AL&page=2&pageSize=3");
            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Factory.ClienteService.Verify(s => s.SearchBySiglaAsync("AL", 2, 3), Times.Once);
        }

        [Test(Description = "TCLIAPI6 - GET /api/clientes/search/by-sigla devolve ProblemDetails quando termo e vazio.")]
        public async Task SearchBySigla_Should_ReturnProblemDetails_When_SearchTermIsBlank()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/clientes/search/by-sigla?searchTerm=%20");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
            Factory.ClienteService.Verify(s => s.SearchBySiglaAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test(Description = "TCLIAPI7 - GET /api/clientes/search/by-name devolve 200 e chama service quando query e valida.")]
        public async Task SearchByName_Should_ReturnOkJson_When_QueryIsValid()
        {
            // ARRANGE
            var result = new PagedResult<ResponseClienteDto>(
                new[] { new ResponseClienteDto { Cliente_id = 7, Nome = "Cliente Alfa", Sigla = "ALF" } },
                1,
                2,
                3);

            Factory.ClienteService
                .Setup(s => s.SearchByNameAsync("alfa", 2, 3))
                .ReturnsAsync(result);

            // ACT
            var response = await Client.GetAsync("/api/clientes/search/by-name?searchTerm=alfa&page=2&pageSize=3");

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Factory.ClienteService.Verify(s => s.SearchByNameAsync("alfa", 2, 3), Times.Once);
        }

        [Test(Description = "TCLIAPI6 - GET /api/clientes/search/by-name devolve ProblemDetails quando termo e vazio.")]
        public async Task SearchByName_Should_ReturnProblemDetails_When_SearchTermIsBlank()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/clientes/search/by-name?searchTerm=%20");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
            Factory.ClienteService.Verify(s => s.SearchByNameAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test(Description = "TCLIAPI8 - DELETE /api/clientes/{id} devolve 204 quando cliente existe.")]
        public async Task DeleteCliente_Should_ReturnNoContent_When_ClienteExists()
        {
            // ARRANGE
            Factory.ClienteService
                .Setup(s => s.GetByIdAsync(12))
                .ReturnsAsync(new ResponseClienteDto { Cliente_id = 12, Nome = "Cliente A" });
            Factory.ClienteService
                .Setup(s => s.DeleteAsync(12))
                .Returns(Task.CompletedTask);

            // ACT
            var response = await Client.DeleteAsync("/api/clientes/12");

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            Factory.ClienteService.Verify(s => s.DeleteAsync(12), Times.Once);
        }
    }
}
