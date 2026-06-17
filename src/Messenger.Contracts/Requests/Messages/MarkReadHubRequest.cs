namespace Messenger.Contracts.Requests.Messages;

public record MarkReadHubRequest(Guid ConversationId, Guid MessageId);