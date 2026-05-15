using Microsoft.AspNetCore.Identity;

namespace BlazorChat.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> SentMessages { get; set; } = [];
        public ICollection<GroupMember> GroupMembership { get; set; } = [];
        public ICollection<Story> stories { get; set; } = [];
    }

}
