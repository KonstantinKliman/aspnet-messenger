namespace Messenger.Contracts.Dtos.Auth;

public record TokenPairDto(string AccessToken, string RefreshToken, DateTimeOffset AccessExpiresAt);