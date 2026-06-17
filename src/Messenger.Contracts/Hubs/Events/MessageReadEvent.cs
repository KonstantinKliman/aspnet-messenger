namespace Messenger.Contracts.Hubs.Events;

public record MessageReadEvent(Guid ConversationId, Guid UserId, Guid MessageId);