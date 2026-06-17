using System.ComponentModel.DataAnnotations;

namespace Messenger.Contracts.Requests.Users;

public record CreateUserRequest(
    [Required, MinLength(3), MaxLength(32), RegularExpression("^[a-zA-Z0-9_]+$")]
    string Username, 
    
    [Required, MinLength(1), MaxLength(64)]
    string DisplayName
);
