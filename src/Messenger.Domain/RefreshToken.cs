namespace Messenger.Domain;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public Guid UserId { get; set; }
    
    public string TokenHash { get; set; } = null!;
    
    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public DateTimeOffset? RevokedAt { get; set; }
    
    public bool IsActive => RevokedAt is null && ExpiresAt > DateTimeOffset.UtcNow;
}