namespace Messenger.Contracts.Requests.Messages;

public record SendMessageHubRequest(Guid ConversationId, string Content);