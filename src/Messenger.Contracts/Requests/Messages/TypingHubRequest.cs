namespace Messenger.Contracts.Requests.Messages;

public record TypingHubRequest(Guid ConversationId, bool IsTyping);