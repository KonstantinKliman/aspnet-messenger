using System.Security.Claims;
using FluentResults;
using Messenger.Contracts.Dtos.Conversations;

namespace Messenger.Contracts.Api.BusinessLogic;

public interface IConversationsService
{
    Task<Result<ConversationDto>> OpenDirectAsync(Guid userId, CancellationToken ct);

    Task<Result<List<ConversationListItemDto>>> ListAsync(CancellationToken ct = default);

    Task<Result<ConversationDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
}