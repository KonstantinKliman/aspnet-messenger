using Messenger.Api.Auth;
using Messenger.Api.BusinessLogic.Abstractions;
using Messenger.Api.BusinessLogic.Services;
using Messenger.Contracts;
using Messenger.Contracts.Api.BusinessLogic;
using Messenger.Contracts.Dtos.Messages;
using Messenger.Contracts.Hubs.Events;
using Messenger.Contracts.Hubs.Interfaces;
using Messenger.Contracts.Requests.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.Api.Hubs;

[Authorize]
public class ChatHub(IMessageService service, ICurrentUserAccessor currentUser) : Hub<IChatClient>, IChatHub
{
    public async Task<MessageDto> SendMessage(SendMessageHubRequest request)
    {
        var senderId = currentUser.UserId;
        var result = await service.SendAsync(
            request.ConversationId, senderId, request.Content, Context.ConnectionAborted);

        if (result.IsFailed)
            throw new HubException(result.Errors[0].Message);

        foreach (var userId in result.Value.Recipients)
            await Clients.User(userId.ToString()).MessageReceived(result.Value.Message!);

        return result.Value.Message!;
    }

    public async Task MarkRead(MarkReadHubRequest request)
    {
        var userId = currentUser.UserId;
        var evt = new MessageReadEvent(request.ConversationId, userId, request.MessageId);
        await Clients.Others.MessageRead(evt);
    }

    public async Task Typing(TypingHubRequest request)
    {
        var userId = currentUser.UserId;
        var evt = new TypingEvent(request.ConversationId, userId, request.IsTyping);
        await Clients.Others.UserTyping(evt);
    }
}