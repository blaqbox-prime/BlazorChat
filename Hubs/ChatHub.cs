using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BlazorChat.Services;
using BlazorChat.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IMessageService _messages;
        private readonly IPresenceService _presence;
        private readonly ApplicationDbContext _db;

        public ChatHub(IMessageService messages, IPresenceService presence, ApplicationDbContext db)
        {
            _messages = messages;
            _presence = presence;
            _db = db;

        }

        // lifecycle methods
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier!;
            //join group channels 
            var groupIds = await _db.GroupMembers
                .Where(gm => gm.UserId == userId)
                .Select(gm => gm.GroupId.ToString())
                .ToListAsync();

            foreach (var groupId in groupIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            }

            //mark user as online
            await _presence.SetOnlineAsync(userId, true);
            //notify contacts
            await Clients.Others.UserPresenceChanged(userId, true);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier!;
            //mark user as offline
            await _presence.SetOnlineAsync(userId, false);
            //notify contacts
            await Clients.Others.UserPresenceChanged(userId, false);
            await base.OnDisconnectedAsync(exception);
        }

        //messaging
        public async Task SendMessage(string recipientId, string content)
        {
            var senderId = Context.UserIdentifier!;
            var message = await _messages.SaveDirectMessageAsync(senderId, recipientId, content);

            //send the message 
            await Clients.User(recipientId).RecieveMessage(message);
            //optionally send to sender as well to confirm it was sent
            await Clients.Caller.RecieveMessage(message);
        }

        //group messaging
        public async Task SendGroupMessage(string groupId, string content)
        {
            var senderId = Context.UserIdentifier!;
            //save the message to the database and get the message dto
            var isMember = await _db.GroupMembers
                .AnyAsync(gm => gm.GroupId == Guid.Parse(groupId) && gm.UserId == senderId);

            if (!isMember) return;
            var message = await _messages.SaveGroupMessageAsync(senderId, groupId, content);
            //send the message to all members of the group except the sender
            await Clients.Group(groupId).RecieveMessage(message);
            //optionally send to sender as well to confirm it was sent
            await Clients.Caller.RecieveMessage(message);
        }

        //typing indicators
        public async Task NotifyTyping(string channelId, bool isTyping)
        {
            var userId = Context.UserIdentifier!;
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return;
            //notify others in the channel that this user is typing or has stopped typing
            await Clients.OthersInGroup(channelId).UserTyping(channelId, user.DisplayName, isTyping);
        }
    }
}
