using BlazorChat.DTOs;

namespace BlazorChat.Services
{
    public interface IMessageService
    {
        Task<MessageDto> SaveDirectMessageAsync(string senderId, string recipientId, string content);
        Task<MessageDto> SaveGroupMessageAsync(string senderId, string groupId, string content);
    }
}
