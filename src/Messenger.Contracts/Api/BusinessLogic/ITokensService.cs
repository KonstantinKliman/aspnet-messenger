using Messenger.Contracts.Dtos.Auth;
using Messenger.Domain;

namespace Messenger.Contracts.Api.BusinessLogic;

public interface ITokensService
{
    Task<TokenPairDto> IssueAsync(ApplicationUser user, CancellationToken ct);

    Task<TokenPairDto?> RefreshAsync(string refreshToken, CancellationToken ct);

    Task RevokeAsync(string refreshToken, CancellationToken ct);
}