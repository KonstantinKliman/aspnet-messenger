using FluentResults;
using Messenger.Api.BusinessLogic.Abstractions;
using Messenger.Api.DataAccess.DbContext;
using Messenger.Contracts.Api.BusinessLogic;
using Messenger.Contracts.Dtos.Messages;
using Messenger.Contracts.Responses.Messages;
using Messenger.Domain;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Api.BusinessLogic.Services;

public class MessageService(AppDbContext context, ICurrentUserAccessor currentUser) : IMessageService
{
    public async Task<Result<SendMessageResponse>> SendAsync(Guid conversationId,
        Guid senderId, string content, CancellationToken ct = default)
    {
        var participants = await context.Participants
            .Where(p => p.ConversationId == conversationId)
            .Select(p => p.UserId)
            .ToListAsync(ct);

        if (participants.Count == 0)
            return Result.Fail("Conversation not found");

        if (!participants.Contains(senderId))
            return Result.Fail("Not a participant");

        var message = new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Content = content.Trim(),
        };
        
        context.Messages.Add(message);
        await context.SaveChangesAsync(ct);

        var dto = new MessageDto(message.Id, message.ConversationId, message.SenderId, message.Content,
            message.CreatedAt);
        
        var recipients = participants.Where(p => p != senderId).ToList();
        return Result.Ok(new SendMessageResponse(dto, recipients));
    }

    public async Task<Result<List<MessageDto>>> GetHistoryAsync(Guid conversationId, int limit, Guid? before, CancellationToken ct = default)
    {
        var isParticipant = await context.Participants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == currentUser.UserId, ct);

        if (!isParticipant)
            return Result.Fail("Conversation not found");
        
        limit = Math.Clamp(limit, 1, 100);

        var query = context.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId);

        if (before is not null)
            query = query.Where(m => m.Id.CompareTo(before.Value) < 0);

        var messages = await query
            .OrderByDescending(m => m.Id)
            .Take(limit)
            .Select(m => new MessageDto(m.Id, m.ConversationId, m.SenderId, m.Content, m.CreatedAt))
            .ToListAsync(ct);

        messages.Reverse();
        return messages;
    }

    public async Task<Result> MarkReadAsync(Guid conversationId, Guid messageId, CancellationToken ct = default)
    {
        var participant = await context.Participants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == currentUser.UserId, ct);
        
        if (participant is null) 
            return Result.Fail("Participant not found");

        if (participant.LastReadMessageId is null ||
            messageId.CompareTo(participant.LastReadMessageId.Value) > 0)
        {
            participant.LastReadMessageId = messageId;
            await context.SaveChangesAsync(ct);
        }

        return Result.Ok();
    }
}