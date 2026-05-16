using BlazorChat.DTOs;

namespace BlazorChat.Models
{
    public class ChatMessageVm
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string SenderId { get; set; } = string.Empty;
        public bool IsOwn { get; set; }
        public bool IsRead { get; set; } = false;

        public static ChatMessageVm FromDto(MessageDto msg, string currentUserId)
        {
            return new ChatMessageVm
            {
                Content = msg.Content,
                SentAt = DateTime.UtcNow,
                SenderId = msg.SenderId ?? string.Empty,
                IsOwn = msg.SenderId == currentUserId,
                IsRead = false 
            };
        }
    }
}
