using FluentResults;
using Messenger.Contracts.Dtos.Messages;
using Messenger.Contracts.Responses.Messages;

namespace Messenger.Contracts.Api.BusinessLogic;

public interface IMessageService
{
    Task<Result<SendMessageResponse>> SendAsync(Guid conversationId, Guid senderId, string content,
        CancellationToken ct = default);
    
    Task<Result<List<MessageDto>>> GetHistoryAsync(Guid conversationId, int limit, Guid? before, CancellationToken ct = default);

    Task<Result> MarkReadAsync(Guid conversationId, Guid messageId, CancellationToken ct = default);
}