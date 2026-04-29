
using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TipMolde.Application.Dtos.MoldeDto;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Integracao.Controller
{
    [TestFixture]
    [Category("Integration")]
    public sealed class MoldeControllerTests : ControllerHttpTestBase
    {
        [Test(Description = "TMOLAPI1 - GET /api/moldes/por-numero devolve ProblemDetails quando numero e vazio.")]
        public async Task GetByNumero_Should_ReturnProblemDetails_When_NumeroIsBlank()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/moldes/por-numero?numero=%20%20");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
            Factory.MoldeService.Verify(s => s.GetByNumeroAsync(It.IsAny<string>()), Times.Never);
        }

        [Test(Description = "TMOLAPI2 - POST /api/moldes devolve 201 quando request e valida.")]
        public async Task Create_Should_ReturnCreatedJson_When_RequestIsValid()
        {
            // ARRANGE
            var created = new ResponseMoldeDto
            {
                MoldeId = 9,
                Numero = "M-001",
                Numero_cavidades = 2,
                TipoPedido = TipoPedido.NOVO_MOLDE
            };

            Factory.MoldeService
                .Setup(s => s.CreateAsync(It.IsAny<CreateMoldeDto>()))
                .ReturnsAsync(created);

            var payload = new
            {
                numero = "M-001",
                numero_cavidades = 2,
                tipoPedido = TipoPedido.NOVO_MOLDE,
                encomendaId = 1,
                quantidade = 1,
                prioridade = 1,
                dataEntregaPrevista = new DateTime(2026, 5, 10, 0, 0, 0, DateTimeKind.Utc)
            };

            // ACT
            var response = await Client.PostAsJsonAsync("/api/moldes", payload);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var body = await response.Content.ReadFromJsonAsync<ResponseMoldeDto>();
            body.Should().BeEquivalentTo(created);
        }
    }
}
