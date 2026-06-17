using Messenger.Contracts.Dtos.Users;
using Messenger.Domain;

namespace Messenger.Contracts.Dtos.Conversations;

public record ConversationDto(
    Guid Id,
    ConversationType Type,
    DateTimeOffset CreatedAt,
    List<UserDto> Participants);