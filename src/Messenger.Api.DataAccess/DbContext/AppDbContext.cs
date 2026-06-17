using Messenger.Api.DataAccess.Entities;
using Messenger.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Api.DataAccess.DbContext;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Message> Messages => Set<Message>();
    
    public DbSet<Conversation> Conversations => Set<Conversation>();
    
    public DbSet<Participant> Participants => Set<Participant>();
    
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    public DbSet<User> DomainUsers => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ApplicationUser>().ToTable("auth_users");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("auth_roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("auth_user_roles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("auth_user_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("auth_user_logins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("auth_user_tokens");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("auth_role_claims");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}