namespace BlazorChat.Services
{
    public class PresenceService : IPresenceService
    {
        Task IPresenceService.SetOnlineAsync(string userId, bool v)
        {
            throw new NotImplementedException();
        }
    }
}