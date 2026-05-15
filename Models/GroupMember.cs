namespace BlazorChat.Models
{
    public class GroupMember
    {
        public Guid GroupId { get; set; }
        public Group? Group { get; set; }

        public required string UserId { get; set; }
        public AppUser? User { get; set; }

        public GroupRole Role { get; set; } = GroupRole.Member;
        public DateTime joinedAt = DateTime.UtcNow;
    }

    public enum GroupRole
    {
        Owner,
        Admin,
        Member
    }
}