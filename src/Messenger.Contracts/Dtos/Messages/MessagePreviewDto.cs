namespace Messenger.Contracts.Dtos.Messages;

public record MessagePreviewDto(
    Guid Id,
    Guid SenderId,
    string Content,
    DateTimeOffset CreatedAt);