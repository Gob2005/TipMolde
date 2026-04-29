using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TipMolde.Application.Dtos.RevisaoDto;

namespace TipMolde.Tests.Integracao.Controller
{
    [TestFixture]
    [Category("Integration")]
    public sealed class RevisaoControllerTests : ControllerHttpTestBase
    {
        [Test(Description = "TREVAPI1 - GET /api/revisoes devolve ProblemDetails quando projetoId e invalido.")]
        public async Task GetByProjeto_Should_ReturnProblemDetails_When_ProjetoIdIsInvalid()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/revisoes?projetoId=0");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
            Factory.RevisaoService.Verify(
                s => s.GetByProjetoIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
        }

        [Test(Description = "TREVAPI2 - POST /api/revisoes devolve 201 quando request e valida.")]
        public async Task Create_Should_ReturnCreatedJson_When_RequestIsValid()
        {
            // ARRANGE
            var created = new ResponseRevisaoDto
            {
                Revisao_id = 3,
                NumRevisao = 1,
                DescricaoAlteracoes = "Ajustar extratores",
                DataEnvioCliente = DateTime.UtcNow,
                Projeto_id = 2
            };

            Factory.RevisaoService
                .Setup(s => s.CreateAsync(It.IsAny<CreateRevisaoDto>()))
                .ReturnsAsync(created);

            // ACT
            var response = await Client.PostAsJsonAsync("/api/revisoes", new { descricaoAlteracoes = "Ajustar extratores", projeto_id = 2 });

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var body = await response.Content.ReadFromJsonAsync<ResponseRevisaoDto>();
            body.Should().BeEquivalentTo(created);
        }

        [Test(Description = "TREVAPI3 - PUT /api/revisoes/{id}/resposta-cliente devolve ProblemDetails quando rejeicao nao tem feedback.")]
        public async Task UpdateRespostaCliente_Should_ReturnProblemDetails_When_RejectionHasNoFeedback()
        {
            // ARRANGE

            // ACT
            var response = await Client.PutAsJsonAsync("/api/revisoes/3/resposta-cliente", new { aprovado = false });

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            Factory.RevisaoService.Verify(
                s => s.UpdateRespostaClienteAsync(It.IsAny<int>(), It.IsAny<UpdateRespostaRevisaoDto>()),
                Times.Never);
        }
    }
}
