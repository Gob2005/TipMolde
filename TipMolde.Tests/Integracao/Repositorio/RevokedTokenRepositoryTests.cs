using FluentAssertions;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio;

[TestFixture]
[Category("Integration")]
public sealed class RevokedTokenRepositoryTests : RepositoryIntegrationTestBase
{
    [Test(Description = "TRTREP1 - RevokeAsync deve guardar token revogado e IsRevokedAsync deve reconhecer JTI.")]
    public async Task RevokeAsyncAndIsRevokedAsync_Should_PersistAndDetectRevokedToken()
    {
        // ARRANGE
        await using var context = CreateContext();
        var repository = new RevokedTokenRepository(context);

        // ACT
        var before = await repository.IsRevokedAsync("jti-1");
        await repository.RevokeAsync("jti-1", DateTime.UtcNow.AddHours(1));
        var after = await repository.IsRevokedAsync("jti-1");

        // ASSERT
        before.Should().BeFalse();
        after.Should().BeTrue();
        context.RevokedTokens.Should().ContainSingle(t => t.Jti == "jti-1");
    }

    [Test(Description = "TRTREP2 - RevokeAsync deve ser idempotente para o mesmo JTI.")]
    public async Task RevokeAsync_Should_NotDuplicateToken_When_JtiAlreadyExists()
    {
        // ARRANGE
        await using var context = CreateContext();
        var repository = new RevokedTokenRepository(context);

        // ACT
        await repository.RevokeAsync("jti-dup", DateTime.UtcNow.AddHours(1));
        await repository.RevokeAsync("jti-dup", DateTime.UtcNow.AddHours(2));

        // ASSERT
        context.RevokedTokens.Should().ContainSingle(t => t.Jti == "jti-dup");
    }
}
