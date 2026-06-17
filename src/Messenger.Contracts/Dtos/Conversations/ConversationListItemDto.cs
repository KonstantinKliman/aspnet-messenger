using Messenger.Contracts.Dtos.Messages;
using Messenger.Contracts.Dtos.Users;

namespace Messenger.Contracts.Dtos.Conversations;

public record ConversationListItemDto(
    Guid Id,
    UserDto? Peer,
    MessagePreviewDto? LastMessage,
    int UnreadCount);