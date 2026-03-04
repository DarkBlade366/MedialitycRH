using Application.Features.Redmine.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Web.API.BackgroundServices
{
    public class RedmineSyncBackgroundService : BackgroundService
    {
        private readonly ILogger<RedmineSyncBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<RedmineSyncScheduleOptions> _options;

        public RedmineSyncBackgroundService(
            ILogger<RedmineSyncBackgroundService> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<RedmineSyncScheduleOptions> options)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var options = _options.Value;

                if (!options.Enabled)
                {
                    _logger.LogInformation("Redmine sync scheduler is disabled.");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    continue;
                }

                _logger.LogInformation(
                    "Redmine sync cycle starting at {UtcNow}.",
                    DateTime.UtcNow);

                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    await SyncProjectsAsync(scope, stoppingToken);

                    await SyncUsersAsync(scope, stoppingToken);

                    await SyncMilestonesAsync(scope, stoppingToken);

                    await SyncTimeEntriesAsync(scope, options, stoppingToken);

                    _logger.LogInformation(
                        "Redmine sync cycle completed successfully at {UtcNow}.",
                        DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during Redmine sync cycle.");
                }

                var interval = TimeSpan.FromHours(
                    Math.Max(options.IntervalHours, 1));

                _logger.LogInformation(
                    "Next Redmine sync in {IntervalHours} hour(s).",
                    interval.TotalHours);

                await Task.Delay(interval, stoppingToken);
            }
        }

        private async Task SyncProjectsAsync(IServiceScope scope, CancellationToken ct)
        {
            try
            {
                var handler = scope.ServiceProvider
                    .GetRequiredService<SyncRedmineProjectsHandler>();

                var created = await handler.Handle(ct);

                _logger.LogInformation(
                    "Redmine projects synced. Created: {Created}.", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync Redmine projects.");
            }
        }

        private async Task SyncUsersAsync(IServiceScope scope, CancellationToken ct)
        {
            try
            {
                var handler = scope.ServiceProvider
                    .GetRequiredService<SyncRedmineUsersHandler>();

                var created = await handler.Handle(ct);

                _logger.LogInformation(
                    "Redmine users synced. Created: {Created}.", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync Redmine users.");
            }
        }

        private async Task SyncMilestonesAsync(IServiceScope scope, CancellationToken ct)
        {
            try
            {
                var handler = scope.ServiceProvider
                    .GetRequiredService<SyncRedmineMilestonesHandler>();

                var created = await handler.Handle(ct);

                _logger.LogInformation(
                    "Redmine milestones synced. Created: {Created}.", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync Redmine milestones.");
            }
        }

        private async Task SyncTimeEntriesAsync(
            IServiceScope scope,
            RedmineSyncScheduleOptions options,
            CancellationToken ct)
        {
            try
            {
                var handler = scope.ServiceProvider
                    .GetRequiredService<SyncRedmineTimeEntriesHandler>();

                var from = DateTime.UtcNow.AddDays(-options.TimeEntryLookBackDays);
                var to = DateTime.UtcNow;

                var created = await handler.Handle(from, to, ct);

                _logger.LogInformation(
                    "Redmine time entries synced ({From} to {To}). Created: {Created}.",
                    from, to, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync Redmine time entries.");
            }
        }
    }
}
