using Messenger.Contracts.Dtos.Users;

namespace Messenger.Contracts.Responses.Auth;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessExpiresAt,
    UserDto User);