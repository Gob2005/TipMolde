

using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TipMolde.Application.Dtos.PecaDto;

namespace TipMolde.Tests.Integracao.Controller
{
    [TestFixture]
    [Category("Integration")]
    public sealed class PecaControllerTests : ControllerHttpTestBase
    {
        [Test(Description = "TPECAAPI1 - GET /api/pecas/por-designacao devolve ProblemDetails quando designacao e vazia.")]
        public async Task GetByDesignacao_Should_ReturnProblemDetails_When_DesignacaoIsBlank()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/pecas/por-designacao?designacao=%20%20&moldeId=1");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
            Factory.PecaService.Verify(s => s.GetByDesignacaoAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Test(Description = "TPECAAPI2 - POST /api/pecas devolve 201 quando request e valida.")]
        public async Task Create_Should_ReturnCreatedJson_When_RequestIsValid()
        {
            // ARRANGE
            var created = new ResponsePecaDto
            {
                PecaId = 11,
                Designacao = "Placa",
                Prioridade = 1,
                Quantidade = 2,
                Molde_id = 5
            };

            Factory.PecaService
                .Setup(s => s.CreateAsync(It.IsAny<CreatePecaDto>()))
                .ReturnsAsync(created);

            var payload = new
            {
                designacao = "Placa",
                prioridade = 1,
                quantidade = 2,
                molde_id = 5
            };

            // ACT
            var response = await Client.PostAsJsonAsync("/api/pecas", payload);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var body = await response.Content.ReadFromJsonAsync<ResponsePecaDto>();
            body.Should().BeEquivalentTo(created);
        }

        /*[Test(Description = "TPECAAPI3 - POST /api/pecas/por-molde/{id}/importacao-csv devolve ProblemDetails quando ficheiro falta.")]
        public async Task ImportarCsv_Should_ReturnProblemDetails_When_FileIsMissing()
        {
            // ARRANGE
            using var form = new MultipartFormDataContent();

            // ACT
            var response = await Client.PostAsync("/api/pecas/por-molde/5/importacao-csv", form);

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
            Factory.PecaService.Verify(s => s.ImportarCsvAsync(It.IsAny<int>(), It.IsAny<Stream>()), Times.Never);
        }*/
    }

}
