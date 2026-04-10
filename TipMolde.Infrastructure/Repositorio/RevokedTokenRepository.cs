using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface.Utilizador.IAuth;
using TipMolde.Domain.Entities;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class RevokedTokenRepository : IRevokedTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RevokedTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<bool> IsRevokedAsync(string jti) =>
            _context.RevokedTokens.AnyAsync(x => x.Jti == jti);

        public async Task RevokeAsync(string jti, DateTime expiresAtUtc)
        {
            var exists = await _context.RevokedTokens.AnyAsync(x => x.Jti == jti);
            if (exists) return;

            await _context.RevokedTokens.AddAsync(new RevokedToken
            {
                Jti = jti,
                ExpiresAtUtc = expiresAtUtc
            });
            await _context.SaveChangesAsync();
        }
    }
}
