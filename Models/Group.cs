namespace BlazorChat.Models
{
    public class Group
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<GroupMember> Members { get; set; } = [];
        public ICollection<Message> Messages { get; set; } = [];
    }
}