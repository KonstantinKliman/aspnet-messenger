namespace Messenger.Contracts.Dtos.Messages;

public record MessageDto(
    Guid Id,
    Guid ConversationId,
    Guid SenderId,
    string Content,
    DateTimeOffset CreatedAt);