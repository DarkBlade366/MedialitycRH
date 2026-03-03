using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Hosting;
using Application.Services;

namespace Web.API.BackgroundServices
{
    public class VacationAccrualBackgroundService : BackgroundService
    {
        private readonly ILogger<VacationAccrualBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public VacationAccrualBackgroundService(
            ILogger<VacationAccrualBackgroundService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<VacationAccrualService>();

                try
                {
                    await service.AccrueVacationsAsync();
                    _logger.LogInformation("Vacations accrued successfully at {Time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error accruing vacation days");
                }

                var now = DateTime.UtcNow;
                var nextRun = new DateTime(now.Year, now.Month, 1).AddMonths(1);
                var delay = nextRun - now;

                if (delay.TotalMilliseconds <= 0)
                    delay = TimeSpan.FromMinutes(1);

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
