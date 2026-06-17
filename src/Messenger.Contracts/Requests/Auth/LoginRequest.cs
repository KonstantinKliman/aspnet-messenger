using System.ComponentModel.DataAnnotations;

namespace Messenger.Contracts.Requests.Auth;

public record LoginRequest(
    [Required] string Username,
    [Required] string Password);