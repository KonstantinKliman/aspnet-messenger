namespace Messenger.Domain;

public class Participant
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public Guid ConversationId { get; set; }
    
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
    
    public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    
    public Guid? LastReadMessageId { get; set; }
}