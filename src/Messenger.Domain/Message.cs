namespace Messenger.Domain;

public class Message
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public Guid ConversationId { get; set; }
    
    public Guid SenderId { get; set; }

    public string Content { get; set; } = null!;
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}