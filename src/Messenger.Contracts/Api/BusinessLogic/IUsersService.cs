using FluentResults;
using Messenger.Contracts.Dtos.Users;

namespace Messenger.Contracts.Api.BusinessLogic;

public interface IUsersService
{
    Task<Result<List<UserDto>>> Search(string query, CancellationToken ct = default);
}