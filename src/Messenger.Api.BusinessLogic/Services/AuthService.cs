using FluentResults;
using Mapster;
using Messenger.Api.DataAccess.DbContext;
using Messenger.Contracts;
using Messenger.Contracts.Api.BusinessLogic;
using Messenger.Contracts.Dtos;
using Messenger.Contracts.Dtos.Auth;
using Messenger.Contracts.Dtos.Users;
using Messenger.Contracts.Responses.Auth;
using Messenger.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Api.BusinessLogic.Services;

public class AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, ITokensService tokensService) : IAuthService
{
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterUserDto dto, CancellationToken ct = default)
    {
        var username = dto.Username.Trim().ToLowerInvariant();

        if (await db.DomainUsers.AnyAsync(u => u.Username == username, ct))
            return Result.Fail("Username already taken");

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var domainUser = new User
        {
            Username = username,
            DisplayName = dto.DisplayName.Trim()
        };
        db.DomainUsers.Add(domainUser);
        await db.SaveChangesAsync(ct);

        var appUser = new ApplicationUser
        {
            UserName = username,
            UserId = domainUser.Id
        };

        var result = await userManager.CreateAsync(appUser, dto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => x.Description);
            return Result.Fail(errors);
        }

        await tx.CommitAsync(ct);

        var pair = await tokensService.IssueAsync(appUser, ct);
        return Result.Ok(pair.Adapt<AuthResponse>());
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginUserDto dto, CancellationToken ct = default)
    {
        var username = dto.Username.Trim().ToLowerInvariant();
        var appUser = await userManager.FindByNameAsync(username);
        if (appUser is null || !await userManager.CheckPasswordAsync(appUser, dto.Password))
            return Result.Fail("Invalid username or password");

        var domainUser = await db.DomainUsers.FindAsync([appUser.UserId], ct);
        if (domainUser is null)
            return Result.Fail("Invalid username or password");

        var pair = await tokensService.IssueAsync(appUser, ct);

        var res = new AuthResponse(pair.AccessToken, pair.RefreshToken, pair.AccessExpiresAt,
            new UserDto(domainUser.Id, domainUser.Username, domainUser.DisplayName, domainUser.CreatedAt));
        return Result.Ok(res);
    }

    public async Task<Result<TokenPairDto?>> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        return await tokensService.RefreshAsync(refreshToken, ct);
    }

    public async Task<Result> LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        await tokensService.RevokeAsync(refreshToken, ct);
        return Result.Ok();
    }
}