using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Messenger.Api.DataAccess.DbContext;
using Messenger.Api.DataAccess.Entities;
using Messenger.Contracts;
using Messenger.Contracts.Api.BusinessLogic;
using Messenger.Contracts.Dtos.Auth;
using Messenger.Contracts.Options;
using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.Api.BusinessLogic.Services;

internal class TokensService(IOptions<JwtOptions> options, AppDbContext db) : ITokensService
{
    private readonly JwtOptions _opts = options.Value;

    public async Task<TokenPairDto> IssueAsync(ApplicationUser user, CancellationToken ct)
    {
        var accessExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_opts.AccessTokenMinutes);
        var access = CreateAccessToken(user, accessExpiresAt);

        var refresh = GenerateRefreshToken();
        var refreshExpiresAt = DateTimeOffset.UtcNow.AddDays(_opts.RefreshTokenDays);

        db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.UserId,
            TokenHash = Hash(refresh),
            ExpiresAt = refreshExpiresAt
        });
        await db.SaveChangesAsync(ct);

        return new TokenPairDto(access, refresh, accessExpiresAt);
    }

    public async Task<TokenPairDto?> RefreshAsync(string refreshToken, CancellationToken ct)
    {
        var hash = Hash(refreshToken);
        var token = await db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
        if (token is null || !token.IsActive) 
            return null;

        token.RevokedAt = DateTimeOffset.UtcNow;

        var appUser = await db.Users.FirstOrDefaultAsync(u => u.Id == token.UserId, ct);
        
        if (appUser is null) 
            return null;

        return await IssueAsync(appUser, ct);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken ct)
    {
        var hash = Hash(refreshToken);
        var token = await db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
        if (token is { RevokedAt: null })
        {
            token.RevokedAt = DateTimeOffset.UtcNow;
            await db.SaveChangesAsync(ct);
        }
    }

    private string CreateAccessToken(ApplicationUser user, DateTimeOffset expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opts.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new Claim("username", user.UserName!)
        };

        var token = new JwtSecurityToken(
            issuer: _opts.Issuer,
            audience: _opts.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}