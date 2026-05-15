namespace BlazorChat.Models
{
    public class StoryView
    {
        public Guid StoryId { get; set; }
        public Story? Story { get; set; }

        public required string ViewerId { get; set; }
        public AppUser? Viewer { get; set; }

        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    }
}