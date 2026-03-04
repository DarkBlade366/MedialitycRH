using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Web.API.BackgroundServices
{
    public class MonthlyPayrollBackgroundService : BackgroundService
    {
        private readonly ILogger<MonthlyPayrollBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<MonthlyPayrollScheduleOptions> _options;

        public MonthlyPayrollBackgroundService(
            ILogger<MonthlyPayrollBackgroundService> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<MonthlyPayrollScheduleOptions> options)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var options = _options.Value;

                if (!options.Enabled)
                {
                    _logger.LogInformation("Monthly payroll scheduler is disabled.");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    continue;
                }

                var nowUtc = DateTime.UtcNow;
                var nextRunUtc = GetNextRunUtc(nowUtc, options);
                var delay = nextRunUtc - nowUtc;

                if (delay.TotalMilliseconds <= 0)
                    delay = TimeSpan.FromSeconds(1);

                _logger.LogInformation(
                    "Monthly payroll scheduler waiting until {NextRunUtc}.",
                    nextRunUtc);

                await Task.Delay(delay, stoppingToken);

                try
                {
                    var executionTimeUtc = DateTime.UtcNow;
                    var periodEnd = new DateTime(
                        executionTimeUtc.Year,
                        executionTimeUtc.Month,
                        1,
                        0, 0, 0,
                        DateTimeKind.Utc);
                    var periodStart = periodEnd.AddMonths(-1);

                    using var scope = _scopeFactory.CreateScope();
                    var payrollService = scope.ServiceProvider
                        .GetRequiredService<MonthlyPayrollService>();

                    var result = await payrollService.GenerateForAllActiveEmployeesAsync(
                        periodStart,
                        periodEnd,
                        stoppingToken);

                    _logger.LogInformation(
                        "Monthly payroll executed for period {PeriodStart} - {PeriodEnd}. " +
                        "TotalEmployees={TotalEmployees}, Created={Created}, Skipped={Skipped}, Failed={Failed}",
                        periodStart,
                        periodEnd,
                        result.TotalEmployees,
                        result.CreatedPayrolls,
                        result.SkippedPayrolls,
                        result.FailedPayrolls);

                    foreach (var error in result.Errors)
                        _logger.LogWarning("Monthly payroll warning: {Error}", error);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing monthly payroll scheduler.");
                }
            }
        }

        private static DateTime GetNextRunUtc(
            DateTime nowUtc,
            MonthlyPayrollScheduleOptions options)
        {
            var runDay = Math.Clamp(options.RunDayOfMonth, 1, 31);
            var runHour = Math.Clamp(options.RunHourUtc, 0, 23);
            var runMinute = Math.Clamp(options.RunMinuteUtc, 0, 59);

            var currentMonthRun = BuildMonthlyRunUtc(
                nowUtc.Year,
                nowUtc.Month,
                runDay,
                runHour,
                runMinute);

            if (nowUtc < currentMonthRun)
                return currentMonthRun;

            var nextMonth = nowUtc.AddMonths(1);

            return BuildMonthlyRunUtc(
                nextMonth.Year,
                nextMonth.Month,
                runDay,
                runHour,
                runMinute);
        }

        private static DateTime BuildMonthlyRunUtc(
            int year,
            int month,
            int day,
            int hour,
            int minute)
        {
            var lastDayInMonth = DateTime.DaysInMonth(year, month);
            var safeDay = Math.Min(day, lastDayInMonth);

            return new DateTime(
                year,
                month,
                safeDay,
                hour,
                minute,
                0,
                DateTimeKind.Utc);
        }
    }
}
