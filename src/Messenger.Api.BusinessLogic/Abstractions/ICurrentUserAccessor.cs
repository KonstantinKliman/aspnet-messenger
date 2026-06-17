namespace Messenger.Api.BusinessLogic.Abstractions;

public interface ICurrentUserAccessor
{
    Guid UserId { get; }
}