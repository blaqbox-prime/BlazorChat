namespace BlazorChat.Services
{
    public interface IPresenceService
    {
        Task SetOnlineAsync(string userId, bool v);
    }
}
