using FluentResults;
using Mapster;
using Messenger.Api.BusinessLogic.Abstractions;
using Messenger.Api.DataAccess.DbContext;
using Messenger.Contracts;
using Messenger.Contracts.Api.BusinessLogic;
using Messenger.Contracts.Dtos.Conversations;
using Messenger.Contracts.Dtos.Messages;
using Messenger.Contracts.Dtos.Users;
using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using ConversationType = Messenger.Domain.ConversationType;

namespace Messenger.Api.BusinessLogic.Services;

public class ConversationsService(AppDbContext db, ICurrentUserAccessor currentUser) : IConversationsService
{
    public async Task<Result<ConversationDto>> OpenDirectAsync(Guid userId, CancellationToken ct)
    {
        var currentUserId = currentUser.UserId;
        if (userId == currentUserId)
            return Result.Fail("Cannot open a direct conversation with yourself");

        var peer = await db.DomainUsers.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (peer is null) 
            return Result.Fail("User not found");
        
        var existing = await db.Conversations
            .Where(c => c.Type == ConversationType.Direct)
            .Where(c => c.Participants.Any(p => p.UserId == currentUserId))
            .Where(c => c.Participants.Any(p => p.UserId == userId))
            .Select(c => c.Id)
            .FirstOrDefaultAsync(ct);

        if (existing != Guid.Empty)
            return Result.Ok(await BuildDto(existing, ct));

        var conversation = new Conversation
        {
            Type = ConversationType.Direct,
            Participants =
            {
                new Participant { UserId = currentUserId },
                new Participant { UserId = userId }
            }
        };
        
        db.Conversations.Add(conversation);
        await db.SaveChangesAsync(ct);
        
        var res = await BuildDto(conversation.Id, ct);

        return Result.Ok(res);
    }

    public async Task<Result<List<ConversationListItemDto>>> ListAsync(CancellationToken ct = default)
    {
        var currentUserId = currentUser.UserId;
        
        var conversations = await db.Conversations
            .Where(c => c.Participants.Any(p => p.UserId == currentUserId))
            .Select(c => new
            {
                c.Id,
                c.Type,
                c.CreatedAt,
                Peer = c.Participants
                    .Where(p => p.UserId != currentUserId)
                    .Select(p => db.DomainUsers.First(u => u.Id == p.UserId))
                    .FirstOrDefault(),
                LastMessage = db.Messages
                    .Where(m => m.ConversationId == c.Id)
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => new { m.Id, m.Content, m.CreatedAt, m.SenderId })
                    .FirstOrDefault(),
                MyLastReadId = c.Participants
                    .Where(p => p.UserId == currentUserId)
                    .Select(p => p.LastReadMessageId)
                    .FirstOrDefault(),
                UnreadCount = db.Messages.Count(m =>
                    m.ConversationId == c.Id &&
                    m.SenderId != currentUserId &&
                    (c.Participants.Where(p => p.UserId == currentUserId).Select(p => p.LastReadMessageId).First() == null
                     || m.Id.CompareTo(c.Participants.Where(p => p.UserId == currentUserId).Select(p => p.LastReadMessageId).First()!.Value) > 0))
            })
            .ToListAsync(ct);

        var result = conversations
            .OrderByDescending(c => c.LastMessage?.CreatedAt ?? c.CreatedAt)
            .Select(c => new ConversationListItemDto(
                c.Id,
                c.Peer is null ? null : new UserDto(c.Peer.Id, c.Peer.Username, c.Peer.DisplayName, c.Peer.CreatedAt),
                c.LastMessage is null
                    ? null
                    : new MessagePreviewDto(c.LastMessage.Id, c.LastMessage.SenderId, c.LastMessage.Content, c.LastMessage.CreatedAt),
                c.UnreadCount))
            .ToList();

        return Result.Ok(result);
    }

    public async Task<Result<ConversationDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var currentUserId = currentUser.UserId;
        var isParticipant = await db.Participants
            .AnyAsync(p => p.ConversationId == id && p.UserId == currentUserId, ct);
        if (!isParticipant)
            return Result.Fail("Conversation not found");
        
        return Result.Ok(await BuildDto(id, ct));
    }

    private async Task<ConversationDto> BuildDto(Guid conversationId, CancellationToken ct)
    {
        return await db.Conversations
            .AsNoTracking()
            .Include(c => c.Participants)
                .ThenInclude(p => p.User)
            .Where(c => c.Id == conversationId)
            .ProjectToType<ConversationDto>()
            .FirstAsync(ct);
    }
}