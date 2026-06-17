using Messenger.Contracts.Dtos.Messages;
using Messenger.Contracts.Requests.Messages;

namespace Messenger.Contracts.Hubs.Interfaces;

public interface IChatHub
{
    Task<MessageDto> SendMessage(SendMessageHubRequest request);
    Task MarkRead(MarkReadHubRequest request);
    Task Typing(TypingHubRequest request);
}