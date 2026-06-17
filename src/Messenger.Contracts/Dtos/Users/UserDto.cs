namespace Messenger.Contracts.Dtos.Users;

public record UserDto(
    Guid Id,
    string Username,
    string DisplayName,
    DateTimeOffset CreatedAt
);