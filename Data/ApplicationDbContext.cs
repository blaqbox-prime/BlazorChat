using BlazorChat.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorChat.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Story> Stories => Set<Story>();
        public DbSet<StoryView> StoryViews => Set<StoryView>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //make composite pk for GroupMember table
            builder.Entity<GroupMember>().HasKey(gm => new { gm.GroupId, gm.UserId })
            //make composite pk for story view table (story + userid as viewerId)
            builder.Entity<StoryView>().HasKey(sv => new { sv.StoryId, sv.ViewerId });
            //index messages by date sent
            builder.Entity<Message>().HasIndex(m => m.SentAt);
            //filter out expired stories (older than 24 hours) by default
            builder.Entity<Story>().HasQueryFilter(s => s.ExpiresAt > DateTime.UtcNow);
            //prevent cascading deletes
            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
