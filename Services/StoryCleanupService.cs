using BlazorChat.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorChat.Services
{
    public class StoryCleanupService : BackgroundService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly ILogger<StoryCleanupService> _logger;

        public StoryCleanupService(IDbContextFactory<ApplicationDbContext> factory,
            ILogger<StoryCleanupService> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Run cleanup once per hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

                try
                {
                    await using var db = await _factory.CreateDbContextAsync(stoppingToken);

                    // Must use IgnoreQueryFilters to see expired stories
                    var deleted = await db.Stories
                        .IgnoreQueryFilters()
                        .Where(s => s.ExpiresAt <= DateTime.UtcNow)
                        .ExecuteDeleteAsync(stoppingToken);

                    _logger.LogInformation("Cleaned up {Count} expired stories.", deleted);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Story cleanup failed.");
                }
            }
        }
    }

    

}
