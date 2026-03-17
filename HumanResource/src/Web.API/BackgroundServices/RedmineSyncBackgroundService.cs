using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Application.Common.Interfaces;

namespace Web.API.BackgroundServices
{
    public class RedmineSyncBackgroundService : BackgroundService
    {
        private readonly ILogger<RedmineSyncBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RedmineSyncScheduleOptions _options;
    
        public RedmineSyncBackgroundService(
            ILogger<RedmineSyncBackgroundService> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<RedmineSyncScheduleOptions> options)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
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
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var job = scope.ServiceProvider.GetRequiredService<IRedmineSyncJob>();
                        await job.ExecuteAsync(stoppingToken);
                    }
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

