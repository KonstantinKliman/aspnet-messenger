using System.ComponentModel.DataAnnotations;

namespace Messenger.Contracts.Requests.Messages;

// TODO : FluentValidation
public record SendMessageRequest(
    [Required, MinLength(1), MaxLength(4000)] string Content);