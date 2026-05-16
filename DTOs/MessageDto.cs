using BlazorChat.Models;

namespace BlazorChat.DTOs
{
    public class MessageDto
    {
        public required string Content { get; set; }
       
        public required string? SenderId { get; set; }

        public string? RecipientId { get; set; }
     
        public string? GroupId { get; set; }

    }
}
