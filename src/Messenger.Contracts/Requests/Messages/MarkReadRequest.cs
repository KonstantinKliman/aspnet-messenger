using System.ComponentModel.DataAnnotations;

namespace Messenger.Contracts.Requests.Messages;

public record MarkReadRequest([Required] Guid MessageId);