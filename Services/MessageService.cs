using BlazorChat.DTOs;
using BlazorChat.Services;
using BlazorChat.Models;
using Microsoft.EntityFrameworkCore;
using BlazorChat.Data;

public class MessageService : IMessageService
{

    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    public MessageService(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    Task<MessageDto> IMessageService.SaveGroupMessageAsync(string senderId, string groupId, string content)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<ChatMessageVm>> GetDirectMessagesAsync(string userId, string partnerId, int page = 0, int pageSize = 50)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Messages
            .Where(m =>
            (m.SenderId == Guid.Parse(userId) && m.RecipientId == partnerId) ||
            (m.SenderId == Guid.Parse(partnerId) && m.RecipientId == userId))
            .OrderByDescending(m => m.SentAt)
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(m => new ChatMessageVm
            {
                Id = m.Id,
                Content = m.Content,
                SentAt = m.SentAt,
                SenderId = m.SenderId.ToString(),
                IsOwn = m.SenderId == Guid.Parse(userId),
                IsRead = m.IsRead
            })
            .ToListAsync();

    }

    async Task<MessageDto> IMessageService.SaveDirectMessageAsync(string senderId, string recipientId, string content)
    {
       await using var db = await _factory.CreateDbContextAsync();
        var message = new Message
        {
            Content = content,
            SentAt = DateTime.UtcNow,
            IsRead = false,
            IsDeleted = false,
            SenderId = Guid.Parse(senderId),
            RecipientId = recipientId
        };

        db.Messages.Add(message);
        await db.SaveChangesAsync();
        return MessageDto.From(message);
    }

    public async Task MarkAsReadAsync(string senderId, string recipientId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        await db.Messages
            .Where(m => m.SenderId == Guid.Parse(senderId) && m.RecipientId == recipientId && !m.IsRead)
            .ExecuteUpdateAsync(m => m.SetProperty(msg => msg.IsRead, true));
    }
}