using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TipMolde.Application.Dtos.FichaProducaoDto;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Integracao.Controller
{
    /// <summary>
    /// Testes de integracao HTTP do controller de FichaProducao.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    public sealed class FichaProducaoControllerTests : ControllerHttpTestBase
    {
        [Test(Description = "TFPAPI001 - GET /api/fichas-producao/by-encomendamolde devolve ProblemDetails quando a paginacao e invalida.")]
        public async Task GetByEncomendaMoldeId_Should_ReturnProblemDetails_When_PaginationIsInvalid()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/fichas-producao/by-encomendamolde?encomendaMoldeId=3&page=0&pageSize=10");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
            Factory.FichaProducaoService.Verify(
                s => s.GetByEncomendaMoldeIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
        }

        [Test(Description = "TFPAPI002 - POST /api/fichas-producao/create devolve 201 com a ficha criada quando o request e valido.")]
        public async Task Create_Should_ReturnCreatedJson_When_RequestIsValid()
        {
            // ARRANGE
            var created = BuildFichaResponse(id: 12, tipo: TipoFicha.FRE);

            Factory.FichaProducaoService
                .Setup(s => s.CreateAsync(It.IsAny<CreateFichaProducaoDto>()))
                .ReturnsAsync(created);

            var payload = new
            {
                tipo = TipoFicha.FRE,
                encomendaMolde_id = 7
            };

            // ACT
            var response = await Client.PostAsJsonAsync("/api/fichas-producao/create", payload);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var body = await response.Content.ReadFromJsonAsync<ResponseFichaProducaoDto>();
            body.Should().BeEquivalentTo(created);
        }

        [Test(Description = "TFPAPI003 - POST /api/fichas-producao/{id}/submit usa o utilizador autenticado e devolve 200.")]
        public async Task Submit_Should_ReturnOkJson_When_RequestIsValid()
        {
            // ARRANGE
            var updated = BuildFichaResponse(id: 5, tipo: TipoFicha.FLT, estado: EstadoFichaProducao.SUBMETIDA);

            Factory.FichaProducaoService
                .Setup(s => s.SubmitAsync(5, 1))
                .ReturnsAsync(updated);

            // ACT
            var response = await Client.PostAsync("/api/fichas-producao/5/submit", content: null);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<ResponseFichaProducaoDto>();
            body.Should().BeEquivalentTo(updated);
            Factory.FichaProducaoService.Verify(s => s.SubmitAsync(5, 1), Times.Once);
        }

        [Test(Description = "TFPAPI004 - POST /api/fichas-producao/{id}/cancel cancela logicamente a ficha quando o request e valido.")]
        public async Task Cancel_Should_ReturnOkJson_When_RequestIsValid()
        {
            // ARRANGE
            var updated = BuildFichaResponse(id: 9, tipo: TipoFicha.FRE, estado: EstadoFichaProducao.CANCELADA);

            Factory.FichaProducaoService
                .Setup(s => s.CancelAsync(9, 1))
                .ReturnsAsync(updated);

            // ACT
            var response = await Client.PostAsync("/api/fichas-producao/9/cancel", content: null);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<ResponseFichaProducaoDto>();
            body.Should().BeEquivalentTo(updated);
            Factory.FichaProducaoService.Verify(s => s.CancelAsync(9, 1), Times.Once);
        }

        private static ResponseFichaProducaoDto BuildFichaResponse(
            int id,
            TipoFicha tipo,
            EstadoFichaProducao estado = EstadoFichaProducao.RASCUNHO) => new()
            {
                FichaProducao_id = id,
                Tipo = tipo,
                Estado = estado,
                DataCriacao = DateTime.UtcNow,
                SubmetidaEm = estado == EstadoFichaProducao.SUBMETIDA ? DateTime.UtcNow : null,
                Ativa = estado != EstadoFichaProducao.CANCELADA,
                EncomendaMolde_id = 7
            };
    }
}
