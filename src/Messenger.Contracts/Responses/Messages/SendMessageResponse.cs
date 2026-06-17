using Messenger.Contracts.Dtos.Messages;

namespace Messenger.Contracts.Responses.Messages;

public record SendMessageResponse(MessageDto? Message, List<Guid> Recipients);