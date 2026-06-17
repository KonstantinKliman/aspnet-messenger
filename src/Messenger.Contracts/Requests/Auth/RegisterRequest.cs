using System.ComponentModel.DataAnnotations;

namespace Messenger.Contracts.Requests.Auth;


// TODO: Вынести в FluentValidation
public record RegisterRequest(
    [Required, MinLength(3), MaxLength(32), RegularExpression("^[a-zA-Z0-9_]+$")]
    string Username,
    [Required, MinLength(1), MaxLength(64)]
    string DisplayName,
    [Required, MinLength(8), MaxLength(128)]
    string Password);