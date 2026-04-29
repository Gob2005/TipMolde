using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TipMolde.Application.Dtos.FornecedorDto;
using TipMolde.Application.Interface;

namespace TipMolde.Tests.Integracao.Controller
{
    [TestFixture]
    [Category("Integration")]
    public sealed class FornecedorControllerTests : ControllerHttpTestBase
    {
        [Test(Description = "TFORAPI1 - GET /api/fornecedores devolve ProblemDetails quando paginacao e invalida.")]
        public async Task GetAll_Should_ReturnProblemDetails_When_PaginationIsInvalid()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/fornecedores?page=1&pageSize=0");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
            Factory.FornecedorService.Verify(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test(Description = "TFORAPI2 - GET /api/fornecedores/search/by-name devolve ProblemDetails quando termo e vazio.")]
        public async Task SearchByName_Should_ReturnProblemDetails_When_SearchTermIsBlank()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/fornecedores/search/by-name?searchTerm=%20%20%20");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
        }

        [Test(Description = "TFORAPI3 - POST /api/fornecedores devolve 201 e JSON do fornecedor criado quando request e valida.")]
        public async Task Create_Should_ReturnCreatedJson_When_RequestIsValid()
        {
            // ARRANGE
            var created = new ResponseFornecedorDto
            {
                FornecedorId = 8,
                Nome = "Fornecedor A",
                NIF = "987654321"
            };

            Factory.FornecedorService
                .Setup(s => s.CreateAsync(It.IsAny<CreateFornecedorDto>()))
                .ReturnsAsync(created);

            var payload = new
            {
                nome = "Fornecedor A",
                nif = "987654321",
                morada = "Rua Industrial 10"
            };

            // ACT
            var response = await Client.PostAsJsonAsync("/api/fornecedores", payload);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var body = await response.Content.ReadFromJsonAsync<ResponseFornecedorDto>();
            body.Should().BeEquivalentTo(created);
        }

        [Test(Description = "TFORAPI4 - GET /api/fornecedores/{id} devolve 404 quando fornecedor nao existe.")]
        public async Task GetById_Should_ReturnProblemDetails_When_FornecedorDoesNotExist()
        {
            // ARRANGE
            Factory.FornecedorService
                .Setup(s => s.GetByIdAsync(44))
                .ReturnsAsync((ResponseFornecedorDto?)null);

            // ACT
            var response = await Client.GetAsync("/api/fornecedores/44");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.NotFound, "Recurso nao encontrado");
        }

        [Test(Description = "TFORAPI5 - GET /api/fornecedores/search/by-name devolve 200 quando termo e valido.")]
        public async Task SearchByName_Should_ReturnOkJson_When_QueryIsValid()
        {
            // ARRANGE
            var result = new PagedResult<ResponseFornecedorDto>(
                new[] { new ResponseFornecedorDto { FornecedorId = 8, Nome = "Fornecedor A" } },
                1,
                2,
                3);

            Factory.FornecedorService
                .Setup(s => s.SearchByNameAsync("metal", 2, 3))
                .ReturnsAsync(result);

            // ACT
            var response = await Client.GetAsync("/api/fornecedores/search/by-name?searchTerm=metal&page=2&pageSize=3");

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            Factory.FornecedorService.Verify(s => s.SearchByNameAsync("metal", 2, 3), Times.Once);
        }

        [Test(Description = "TFORAPI6 - PUT /api/fornecedores/{id} devolve 204 quando request e valida.")]
        public async Task Update_Should_ReturnNoContent_When_RequestIsValid()
        {
            // ARRANGE
            Factory.FornecedorService
                .Setup(s => s.UpdateAsync(8, It.IsAny<UpdateFornecedorDto>()))
                .Returns(Task.CompletedTask);

            // ACT
            var response = await Client.PutAsJsonAsync("/api/fornecedores/8", new { nome = "Fornecedor Atualizado" });

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            Factory.FornecedorService.Verify(s => s.UpdateAsync(8, It.IsAny<UpdateFornecedorDto>()), Times.Once);
        }

        [Test(Description = "TFORAPI7 - DELETE /api/fornecedores/{id} devolve 204 quando request e valida.")]
        public async Task Delete_Should_ReturnNoContent_When_RequestIsValid()
        {
            // ARRANGE
            Factory.FornecedorService
                .Setup(s => s.DeleteAsync(8))
                .Returns(Task.CompletedTask);

            // ACT
            var response = await Client.DeleteAsync("/api/fornecedores/8");

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            Factory.FornecedorService.Verify(s => s.DeleteAsync(8), Times.Once);
        }
    }

}
