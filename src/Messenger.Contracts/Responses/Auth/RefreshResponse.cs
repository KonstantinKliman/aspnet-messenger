namespace Messenger.Contracts.Responses.Auth;

public record RefreshResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessExpiresAt);