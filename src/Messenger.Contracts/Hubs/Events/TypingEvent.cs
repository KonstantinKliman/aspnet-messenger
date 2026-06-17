namespace Messenger.Contracts.Hubs.Events;

public record TypingEvent(Guid ConversationId, Guid UserId, bool IsTyping);