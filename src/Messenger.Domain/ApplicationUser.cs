using Microsoft.AspNetCore.Identity;

namespace Messenger.Domain;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid UserId { get; set; }
}