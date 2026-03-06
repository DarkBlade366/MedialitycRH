using Application.Features.Redmine.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Web.API.BackgroundServices
{
    public class RedmineSyncBackgroundService : BackgroundService
    {
        private const int ConsecutiveFailureAlertThreshold = 3;

        private readonly ILogger<RedmineSyncBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<RedmineSyncScheduleOptions> _options;
        private int _consecutiveFailures;

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

                var cycleFailed = false;

                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var projectsOk = await SyncProjectsAsync(scope, stoppingToken);
                    var usersOk = await SyncUsersAsync(scope, stoppingToken);
                    var milestonesOk = await SyncMilestonesAsync(scope, stoppingToken);
                    var timeEntriesOk = await SyncTimeEntriesAsync(scope, options, stoppingToken);

                    cycleFailed = !projectsOk || !usersOk || !milestonesOk || !timeEntriesOk;

                    if (!cycleFailed)
                    {
                        _consecutiveFailures = 0;
                        _logger.LogInformation(
                            "Redmine sync cycle completed successfully at {UtcNow}.",
                            DateTime.UtcNow);
                    }
                }
                catch (Exception ex)
                {
                    cycleFailed = true;
                    _logger.LogError(ex,
                        "Error during Redmine sync cycle. ExceptionType={ExceptionType}, Message={Message}",
                        ex.GetType().Name, ex.Message);
                }

                if (cycleFailed)
                {
                    _consecutiveFailures++;
                    if (_consecutiveFailures == ConsecutiveFailureAlertThreshold)
                    {
                        _logger.LogCritical(
                            "ALERTA: Redmine ha fallado {Count} veces consecutivas. Revisar conectividad y configuración de la API.",
                            _consecutiveFailures);
                    }
                }

                var interval = TimeSpan.FromHours(
                    Math.Max(options.IntervalHours, 1));

                _logger.LogInformation(
                    "Next Redmine sync in {IntervalHours} hour(s).",
                    interval.TotalHours);

                await Task.Delay(interval, stoppingToken);
            }
        }

        private async Task<bool> SyncProjectsAsync(IServiceScope scope, CancellationToken ct)
        {
            try
            {
                var handler = scope.ServiceProvider
                    .GetRequiredService<SyncRedmineProjectsHandler>();

                var created = await handler.Handle(ct);

                _logger.LogInformation(
                    "Redmine projects synced. Created: {Created}.", created);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to sync Redmine projects. ExceptionType={ExceptionType}, Message={Message}",
                    ex.GetType().Name, ex.Message);
                return false;
            }
        }

        private async Task<bool> SyncUsersAsync(IServiceScope scope, CancellationToken ct)
        {
            try
            {
                var handler = scope.ServiceProvider
                    .GetRequiredService<SyncRedmineUsersHandler>();

                var created = await handler.Handle(ct);

                _logger.LogInformation(
                    "Redmine users synced. Created: {Created}.", created);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to sync Redmine users. ExceptionType={ExceptionType}, Message={Message}",
                    ex.GetType().Name, ex.Message);
                return false;
            }
        }

        private async Task<bool> SyncMilestonesAsync(IServiceScope scope, CancellationToken ct)
        {
            try
            {
                var handler = scope.ServiceProvider
                    .GetRequiredService<SyncRedmineMilestonesHandler>();

                var created = await handler.Handle(ct);

                _logger.LogInformation(
                    "Redmine milestones synced. Created: {Created}.", created);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to sync Redmine milestones. ExceptionType={ExceptionType}, Message={Message}",
                    ex.GetType().Name, ex.Message);
                return false;
            }
        }

        private async Task<bool> SyncTimeEntriesAsync(
            IServiceScope scope,
            RedmineSyncScheduleOptions options,
            CancellationToken ct)
        {
            var from = DateTime.UtcNow.AddDays(-options.TimeEntryLookBackDays);
            var to = DateTime.UtcNow;

            try
            {
                var handler = scope.ServiceProvider
                    .GetRequiredService<SyncRedmineTimeEntriesHandler>();

                var created = await handler.Handle(from, to, ct);

                _logger.LogInformation(
                    "Redmine time entries synced ({From} to {To}). Created: {Created}.",
                    from, to, created);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to sync Redmine time entries (From={From}, To={To}). ExceptionType={ExceptionType}, Message={Message}",
                    from, to, ex.GetType().Name, ex.Message);
                return false;
            }
        }
    }
}
