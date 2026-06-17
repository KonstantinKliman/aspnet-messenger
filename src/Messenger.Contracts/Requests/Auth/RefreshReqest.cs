using System.ComponentModel.DataAnnotations;

namespace Messenger.Contracts.Requests.Auth;

public record RefreshRequest([Required] string RefreshToken);