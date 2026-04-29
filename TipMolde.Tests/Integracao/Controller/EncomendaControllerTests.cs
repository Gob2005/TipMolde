using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TipMolde.Application.Dtos.EncomendaDto;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Integracao.Controller
{
    [TestFixture]
    [Category("Integration")]
    public sealed class EncomendaControllerTests : ControllerHttpTestBase
    {
        [Test(Description = "TENCAPI1 - GET /api/encomendas devolve ProblemDetails quando paginacao e invalida.")]
        public async Task GetAllEncomendas_Should_ReturnProblemDetails_When_PaginationIsInvalid()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/encomendas?page=0&pageSize=10");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
            Factory.EncomendaService.Verify(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test(Description = "TENCAPI2 - GET /api/encomendas/por-numero-cliente devolve ProblemDetails quando numero e vazio.")]
        public async Task GetByNumeroCliente_Should_ReturnProblemDetails_When_NumeroIsBlank()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/encomendas/por-numero-cliente?numero=%20%20");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
        }

        [Test(Description = "TENCAPI3 - POST /api/encomendas devolve 201 e JSON da encomenda criada quando request e valida.")]
        public async Task CreateEncomenda_Should_ReturnCreatedJson_When_RequestIsValid()
        {
            // ARRANGE
            var created = BuildEncomenda(id: 22);
            Factory.EncomendaService
                .Setup(s => s.CreateAsync(It.IsAny<CreateEncomendaDto>()))
                .ReturnsAsync(created);

            var payload = new
            {
                cliente_id = 3,
                numeroEncomendaCliente = "ENC-001"
            };

            // ACT
            var response = await Client.PostAsJsonAsync("/api/encomendas", payload);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var body = await response.Content.ReadFromJsonAsync<ResponseEncomendaDto>();
            body.Should().BeEquivalentTo(created);
        }

        [Test(Description = "TENCAPI4 - PATCH /api/encomendas/{id}/estado devolve 204 quando request e valida.")]
        public async Task UpdateEstado_Should_ReturnNoContent_When_RequestIsValid()
        {
            // ARRANGE
            Factory.EncomendaService
                .Setup(s => s.UpdateEstadoAsync(22, It.IsAny<UpdateEstadoEncomendaDto>()))
                .Returns(Task.CompletedTask);

            // ACT
            var response = await Client.PatchAsJsonAsync("/api/encomendas/22/estado", new { estado = EstadoEncomenda.EM_PRODUCAO });

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            Factory.EncomendaService.Verify(s => s.UpdateEstadoAsync(22, It.IsAny<UpdateEstadoEncomendaDto>()), Times.Once);
        }

        private static ResponseEncomendaDto BuildEncomenda(int id = 1)
        {
            return new ResponseEncomendaDto
            {
                Encomenda_id = id,
                Cliente_id = 3,
                NumeroEncomendaCliente = "ENC-001",
                DataRegisto = DateTime.UtcNow,
                Estado = EstadoEncomenda.CONFIRMADA
            };
        }
    }
}
