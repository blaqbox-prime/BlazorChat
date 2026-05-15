using BlazorChat.DTOs;
using BlazorChat.Services;

public class MessageService : IMessageService
{
    Task<MessageDto> IMessageService.SaveDirectMessageAsync(string senderId, string recipientId, string content)
    {
        throw new NotImplementedException();
    }

    Task<MessageDto> IMessageService.SaveGroupMessageAsync(string senderId, string groupId, string content)
    {
        throw new NotImplementedException();
    }
}