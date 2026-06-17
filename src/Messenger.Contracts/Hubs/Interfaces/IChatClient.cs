using Messenger.Contracts.Dtos.Messages;
using Messenger.Contracts.Hubs.Events;

namespace Messenger.Contracts.Hubs.Interfaces;

public interface IChatClient
{
    Task MessageReceived(MessageDto message);
    Task MessageRead(MessageReadEvent evt);
    Task UserTyping(TypingEvent evt);
}