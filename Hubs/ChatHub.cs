using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BlazorChat.Services;
using BlazorChat.Data;
using Microsoft.EntityFrameworkCore;
using BlazorChat.Models;
using BlazorChat.DTOs;

namespace BlazorChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IMessageService _messages;
        private readonly IPresenceService _presence;
        private readonly ApplicationDbContext _db;
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public ChatHub(IMessageService messages, IPresenceService presence, ApplicationDbContext db, IDbContextFactory<ApplicationDbContext> factory)
        {
            _messages = messages;
            _presence = presence;
            _factory = factory;
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
    
        public async Task CreateGroup(string name, List<string> memberIds)
        {
            // get invoking user
            var ownerId = Context.UserIdentifier!;
            await using var db = await _factory.CreateDbContextAsync();
            //create the group
            var group = new Group
            {
                Name = name
            };
            //set the invoking user as the owner
            group.Members.Add(new GroupMember { UserId = ownerId, Role = GroupRole.Owner });
            //add others
            foreach (var memberId in memberIds)
            {
                group.Members.Add(new GroupMember { UserId = memberId, Role = GroupRole.Member});
            }

            db.Groups.Add(group);
            await db.SaveChangesAsync();

            //add creators connection to the group channel
            await Groups.AddToGroupAsync(Context.ConnectionId, group.Id.ToString());
            //notify others of the group
            foreach (var memberId in memberIds)
            {
                await Clients.User(memberId).GroupUpdated(GroupDto.From(group));
            }
        }

        public async Task AddMemberToGroup(string groupId, string memberId)
        {
            var requesterId = Context.UserIdentifier!;
            await using var db = await _factory.CreateDbContextAsync();

            //only admin & owner can add members
            var requesterRole = await db.GroupMembers
                .Where(gm => gm.GroupId == Guid.Parse(groupId) && gm.UserId == requesterId)
                .Select(gm => gm.Role)
                .FirstOrDefaultAsync();

            if (requesterRole == GroupRole.Member) return;
            db.GroupMembers.Add(new GroupMember
            {
                GroupId = Guid.Parse(groupId),
                UserId = memberId,
                Role = GroupRole.Member
            });
            await db.SaveChangesAsync();
            //notify the new member
            var group = await db.Groups.FindAsync(Guid.Parse(groupId));
            await Clients.User(memberId).GroupUpdated(GroupDto.From(group!));
        }
    }
}
