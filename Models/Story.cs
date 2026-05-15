namespace BlazorChat.Models
{
    public class Story
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string MediaUrl { get; set; }
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt => CreatedAt.AddHours(24);

        public required string AuthorId { get; set; }
        public AppUser? Author { get; set; }

        public ICollection<StoryView> Views { get; set; } = [];
    }
}