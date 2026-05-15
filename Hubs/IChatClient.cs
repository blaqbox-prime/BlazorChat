using BlazorChat.DTOs;
namespace BlazorChat.Hubs
{
    public interface IChatClient
    {
        Task RecieveMessage(MessageDto message);
        Task UserTyping(string userId, string userName, bool isTyping);
        Task UserPresenceChanged(string userId, bool isOnline);
        Task GroupUpdated(GroupDto group);
        Task NewStory(StoryDto story);
    }
}
