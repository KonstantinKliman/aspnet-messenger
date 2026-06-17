namespace Messenger.Domain;

public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public string Username { get; set; } = null!;

    public string DisplayName { get; set; } = null!;
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}