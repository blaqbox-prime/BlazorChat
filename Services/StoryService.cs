using BlazorChat.Data;
using BlazorChat.DTOs;
using BlazorChat.Hubs;
using BlazorChat.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlazorChat.Services
{
    public class StoryService : IStoryService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly IHubContext<ChatHub, IChatClient> _hub;

        public StoryService(IDbContextFactory<ApplicationDbContext> factory,
            IHubContext<ChatHub, IChatClient> hub)
        {
            _factory = factory;
            _hub = hub;
        }

        public async Task<StoryDto> PublishStoryAsync(string authorId, string mediaUrl, string? caption)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var story = new Story
            {
                AuthorId = authorId,
                MediaUrl = mediaUrl,
                Caption = caption,
            };

            db.Stories.Add(story);
            await db.SaveChangesAsync();

            // Broadcast to all connected users via IHubContext
            await _hub.Clients.All.NewStory(StoryDto.From(story));

            return StoryDto.From(story);
        }

        public async Task RecordViewAsync(Guid storyId, string viewerId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            // Upsert — ignore if already viewed (composite PK prevents duplicates)
            var view = new StoryView { StoryId = storyId, ViewerId = viewerId };
            db.StoryViews.Add(view);

            try { await db.SaveChangesAsync(); }
            catch (DbUpdateException) { } // Duplicate view — silently ignore
        }

        public async Task<List<StoryDto>> GetActiveStoriesAsync()
        {
            await using var db = await _factory.CreateDbContextAsync();

            // HasQueryFilter auto-excludes expired stories
            return await db.Stories
                .Include(s => s.Author)
                .Include(s => s.Views)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => StoryDto.From(s))
                .ToListAsync();
        }


    }
}