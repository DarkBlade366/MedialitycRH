using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Web.API.BackgroundServices
{
    public class RedmineSyncBackgroundService : BackgroundService
    {
        private readonly ILogger<RedmineSyncBackgroundService> _logger;
        private readonly IRedmineSyncJob _job;
        private readonly RedmineSyncScheduleOptions _options;

        public RedmineSyncBackgroundService(
            ILogger<RedmineSyncBackgroundService> logger,
            IRedmineSyncJob job,
            IOptions<RedmineSyncScheduleOptions> options)
        {
            _logger = logger;
            _job = job;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("Redmine sync scheduler is disabled.");
                return;
            }

            // Pequeño delay inicial para evitar ejecución inmediata al arrancar
            await Task.Delay(TimeSpan.FromSeconds(_options.InitialDelaySeconds), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Redmine sync cycle starting at {UtcNow}.", DateTime.UtcNow);

                try
                {
                    await _job.ExecuteAsync(stoppingToken);
                    _logger.LogInformation("Redmine sync cycle completed successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during Redmine sync cycle.");
                }

                var interval = TimeSpan.FromHours(Math.Max(_options.IntervalHours, 1));
                _logger.LogInformation("Next Redmine sync in {IntervalHours} hour(s).", interval.TotalHours);
                await Task.Delay(interval, stoppingToken);
            }
        }
    }
}