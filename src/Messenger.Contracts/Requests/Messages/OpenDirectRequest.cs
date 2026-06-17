using System.ComponentModel.DataAnnotations;

namespace Messenger.Contracts.Requests.Messages;

public record OpenDirectRequest([Required] Guid UserId);