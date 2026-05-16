using BlazorChat.Models;

namespace BlazorChat.DTOs
{
    public class GroupDto
    {
        public Guid? Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<GroupMember> Members { get; set; } = [];
        public ICollection<Message> Messages { get; set; } = [];

        internal static GroupDto From(Group group)
        {
            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                AvatarUrl = group.AvatarUrl,
                CreatedAt = group.CreatedAt,
                Members = group.Members.ToList(),
                Messages = group.Messages.ToList()
            };
        }
    }
}