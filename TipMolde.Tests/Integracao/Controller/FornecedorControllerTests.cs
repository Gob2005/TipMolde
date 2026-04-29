using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TipMolde.Application.Dtos.FornecedorDto;

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
    }

}
