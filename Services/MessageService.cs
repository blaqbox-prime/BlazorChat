using BlazorChat.DTOs;
using BlazorChat.Services;
using BlazorChat.Models;

public class MessageService : IMessageService
{
    Task<MessageDto> IMessageService.SaveGroupMessageAsync(string senderId, string groupId, string content)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<ChatMessageVm>> GetDirectMessagesAsync(string currentUser, string userId)
    {
        throw new NotImplementedException();
    }

    Task<MessageDto> IMessageService.SaveDirectMessageAsync(string senderId, string recipientId, string content)
    {
       throw new NotImplementedException();
    }
}