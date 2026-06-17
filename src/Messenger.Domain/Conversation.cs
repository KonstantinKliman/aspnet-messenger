namespace Messenger.Domain;

public enum ConversationType
{
    Direct = 1,
    Group = 2 
}

public class Conversation
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public ConversationType Type { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<Participant> Participants { get; set; } = new();
}