using BlazorChat.Models;

namespace BlazorChat.DTOs
{
    public class StoryDto
    {
        public Guid? Id { get; set; }
        public required string MediaUrl { get; set; }
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt => CreatedAt.AddHours(24);

        public required string AuthorId { get; set; }
       

        public ICollection<StoryView> Views { get; set; } = [];

        public static StoryDto From(Story story)
        {
            return new StoryDto
            {
                Id = story.Id,
                MediaUrl = story.MediaUrl,
                Caption = story.Caption,
                CreatedAt = story.CreatedAt,
                AuthorId = story.AuthorId,
                Views = story.Views
            };
        }
    }
}