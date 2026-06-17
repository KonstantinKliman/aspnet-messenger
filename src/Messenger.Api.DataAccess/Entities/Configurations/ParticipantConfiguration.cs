using Messenger.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Messenger.Api.DataAccess.Entities.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("participants");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ConversationId).IsRequired();
        builder.Property(p => p.UserId).IsRequired();
        builder.Property(p => p.JoinedAt).IsRequired();

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.ConversationId, p.UserId }).IsUnique();

        builder.HasIndex(p => p.UserId);
    }
}