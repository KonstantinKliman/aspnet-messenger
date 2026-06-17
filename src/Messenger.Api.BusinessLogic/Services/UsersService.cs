using FluentResults;
using Mapster;
using Messenger.Api.DataAccess.DbContext;
using Messenger.Contracts;
using Messenger.Contracts.Api.BusinessLogic;
using Messenger.Contracts.Dtos.Users;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Api.BusinessLogic.Services;

public class UsersService(AppDbContext db) : IUsersService
{
    public async Task<Result<List<UserDto>>> Search(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Result.Fail("Query must be at least 2 characters");

        query = query.Trim().ToLowerInvariant();

        var users = await db.DomainUsers
            .Where(u => EF.Functions.Like(u.Username, $"{query}%"))
            .OrderBy(u => u.Username)
            .Take(20)
            .ToListAsync(ct);
        
        return Result.Ok(users.Adapt<List<UserDto>>());
    }
}