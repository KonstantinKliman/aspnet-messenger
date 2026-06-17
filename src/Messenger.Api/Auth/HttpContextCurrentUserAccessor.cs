using System.Security.Claims;
using Messenger.Api.BusinessLogic.Abstractions;

namespace Messenger.Api.Auth;

public sealed class HttpContextCurrentUserAccessor(IHttpContextAccessor accessor)
    : ICurrentUserAccessor
{
    public Guid UserId
    {
        get
        {
            if (accessor.HttpContext?.User is { } user)
            {
                var sub = user.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? user.FindFirstValue("sub");
                return Guid.Parse(sub!);
            }
                
        
            throw new InvalidOperationException("Current user is unavailable outside an authenticated HTTP request.");
        }
    }
}