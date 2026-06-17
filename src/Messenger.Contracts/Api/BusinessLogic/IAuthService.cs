using FluentResults;
using Messenger.Contracts.Dtos;
using Messenger.Contracts.Dtos.Auth;
using Messenger.Contracts.Responses.Auth;

namespace Messenger.Contracts.Api.BusinessLogic;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterUserDto dto, CancellationToken ct = default);
    
    Task<Result<AuthResponse>> LoginAsync(LoginUserDto dto, CancellationToken ct = default);

    Task<Result<TokenPairDto?>> RefreshAsync(string refreshToken, CancellationToken ct = default);
    
    Task<Result> LogoutAsync(string refreshToken, CancellationToken ct = default);
}