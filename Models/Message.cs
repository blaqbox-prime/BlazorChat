namespace BlazorChat.Models
{
    public class Message
    {
// message props
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
        //sender props
        public required Guid SenderId { get; set; }
        public AppUser? Sender { get; set; }
        //dm props
        public string? RecipientId { get; set; }
        public AppUser? Recipient { get; set; }

        // group props
        public Guid? GroupId { get; set; }
        public Group? Group {  get; set; }

    }
}