using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TipMolde.Application.Dtos.AuthDto;

namespace TipMolde.Tests.Integracao.Controller
{
    [TestFixture]
    [Category("Integration")]
    public sealed class AuthControllerTests : ControllerHttpTestBase
    {
        [Test(Description = "TAUTHAPI1 - POST /api/Auth/login devolve 200 e token quando credenciais sao validas.")]
        public async Task Login_Should_ReturnOkJson_When_CredentialsAreValid()
        {
            // ARRANGE
            var authResponse = new AuthResponseDto
            {
                Token = "jwt-token",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(1)
            };

            Factory.AuthService
                .Setup(s => s.LoginAsync("admin@tipmolde.pt", "Password123!"))
                .ReturnsAsync(authResponse);

            // ACT
            var response = await Client.PostAsJsonAsync("/api/Auth/login", new
            {
                email = "admin@tipmolde.pt",
                password = "Password123!"
            });

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            body.Should().BeEquivalentTo(authResponse);
        }

        [Test(Description = "TAUTHAPI2 - POST /api/Auth/login devolve 401 quando credenciais sao invalidas.")]
        public async Task Login_Should_ReturnUnauthorizedProblem_When_CredentialsAreInvalid()
        {
            // ARRANGE
            Factory.AuthService
                .Setup(s => s.LoginAsync("admin@tipmolde.pt", "Password123!"))
                .ThrowsAsync(new UnauthorizedAccessException());

            // ACT
            var response = await Client.PostAsJsonAsync("/api/Auth/login", new
            {
                email = "admin@tipmolde.pt",
                password = "Password123!"
            });

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.Unauthorized, "Credenciais invalidas");
        }

        [Test(Description = "TAUTHAPI3 - POST /api/Auth/logout devolve 400 quando o service rejeita o logout.")]
        public async Task Logout_Should_ReturnBadRequestProblem_When_ServiceRejectsLogout()
        {
            // ARRANGE
            Factory.AuthService
                .Setup(s => s.LogoutAsync("Test"))
                .ReturnsAsync(new LogoutResultDto { Success = false, Message = "Token invalido." });

            // ACT
            var response = await Client.PostAsync("/api/Auth/logout", null);

            // ASSERT
            await AssertProblemAsync(response, HttpStatusCode.BadRequest, "Logout invalido");
        }

        [Test(Description = "TAUTHAPI4 - POST /api/Auth/logout devolve 200 quando o token e revogado.")]
        public async Task Logout_Should_ReturnOkJson_When_ServiceAcceptsLogout()
        {
            // ARRANGE
            var logoutResult = new LogoutResultDto { Success = true, Message = "Sessao terminada." };

            Factory.AuthService
                .Setup(s => s.LogoutAsync("Test"))
                .ReturnsAsync(logoutResult);

            // ACT
            var response = await Client.PostAsync("/api/Auth/logout", null);

            // ASSERT
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<LogoutResultDto>();
            body.Should().BeEquivalentTo(logoutResult);
        }
    }
}
