using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TipMolde.Application.Dtos.FichaProducaoDto;

namespace TipMolde.Tests.Integracao.Controller
{
    /// <summary>
    /// Testes de integracao HTTP do controller de linhas manuais das fichas de producao.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    public sealed class FichaProducaoRegistosControllerTests : ControllerHttpTestBase
    {
        [Test(Description = "TFPAPI005 - GET /api/fichas-producao/{fichaId}/linhas-frm devolve ProblemDetails quando a paginacao e invalida.")]
        public async Task GetLinhasFrm_Should_ReturnProblemDetails_When_PaginationIsInvalid()
        {
            // ARRANGE

            // ACT
            var response = await Client.GetAsync("/api/fichas-producao/5/linhas-frm?page=0&pageSize=10");

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Pedido invalido");
            Factory.FichaProducaoService.Verify(
                s => s.GetLinhasFrmAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);
        }

        [Test(Description = "TFPAPI006 - POST /api/fichas-producao/{fichaId}/linhas-frm devolve 201 com a linha criada.")]
        public async Task CreateLinhaFrm_Should_ReturnCreatedJson_When_RequestIsValid()
        {
            // ARRANGE
            var created = new ResponseFichaFrmLinhaDto
            {
                FichaFrmLinha_id = 11,
                FichaFrm_id = 5,
                Data = DateTime.UtcNow.Date,
                Defeito = "Rebarba",
                Pormenor = "Rebarba na zona lateral",
                Responsavel_id = 1,
                CriadoEm = DateTime.UtcNow
            };

            Factory.FichaProducaoService
                .Setup(s => s.CreateLinhaFrmAsync(5, It.IsAny<CreateFichaFrmLinhaDto>()))
                .ReturnsAsync(created);

            var payload = new
            {
                data = DateTime.UtcNow.Date,
                defeito = "Rebarba",
                pormenor = "Rebarba na zona lateral",
                responsavel_id = 1
            };

            // ACT
            var response = await Client.PostAsJsonAsync("/api/fichas-producao/5/linhas-frm", payload);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var body = await response.Content.ReadFromJsonAsync<ResponseFichaFrmLinhaDto>();
            body.Should().BeEquivalentTo(created);
            Factory.FichaProducaoService.Verify(
                s => s.CreateLinhaFrmAsync(5, It.IsAny<CreateFichaFrmLinhaDto>()),
                Times.Once);
        }
    }
}
