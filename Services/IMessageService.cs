using BlazorChat.DTOs;
using BlazorChat.Models;

namespace BlazorChat.Services
{
    public interface IMessageService
    {
        public Task<MessageDto> SaveDirectMessageAsync(string senderId, string recipientId, string content);
        public Task<MessageDto> SaveGroupMessageAsync(string senderId, string groupId, string content);
        public Task<ICollection<ChatMessageVm>> GetDirectMessagesAsync(string userId, string partnerId, int page = 0, int pageSize = 50);
        public Task MarkAsReadAsync(string senderId, string recipientId);
    }
}
