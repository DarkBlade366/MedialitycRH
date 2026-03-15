using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Application.Features.Redmine.Handlers;
using Application.Common.Interfaces;

namespace Infrastructure.BackgroundJobs
{
    public class RedmineSyncJob : IRedmineSyncJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RedmineSyncJob> _logger;

        public RedmineSyncJob(IServiceScopeFactory scopeFactory, ILogger<RedmineSyncJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Redmine sync job starting at {UtcNow}.", DateTime.UtcNow);

            using var scope = _scopeFactory.CreateScope();

            var projectsHandler = scope.ServiceProvider.GetRequiredService<SyncRedmineProjectsHandler>();
            var usersHandler = scope.ServiceProvider.GetRequiredService<SyncRedmineUsersHandler>();
            var milestonesHandler = scope.ServiceProvider.GetRequiredService<SyncRedmineMilestonesHandler>();
            var timeEntriesHandler = scope.ServiceProvider.GetRequiredService<SyncRedmineTimeEntriesHandler>();

            bool projectsOk = true, usersOk = true, milestonesOk = true, timeEntriesOk = true;

            try
            {
                var createdProjects = await projectsHandler.Handle(cancellationToken);
                _logger.LogInformation("Projects synced. Created: {Created}.", createdProjects);
            }
            catch (Exception ex)
            {
                projectsOk = false;
                _logger.LogError(ex, "Failed to sync projects.");
            }

            try
            {
                var createdUsers = await usersHandler.Handle(cancellationToken);
                _logger.LogInformation("Users synced. Created: {Created}.", createdUsers);
            }
            catch (Exception ex)
            {
                usersOk = false;
                _logger.LogError(ex, "Failed to sync users.");
            }

            try
            {
                var createdMilestones = await milestonesHandler.Handle(cancellationToken);
                _logger.LogInformation("Milestones synced. Created: {Created}.", createdMilestones);
            }
            catch (Exception ex)
            {
                milestonesOk = false;
                _logger.LogError(ex, "Failed to sync milestones.");
            }

            try
            {
                var from = DateTime.UtcNow.AddDays(-30);
                var to = DateTime.UtcNow;
                var createdTimeEntries = await timeEntriesHandler.Handle(from, to, cancellationToken);
                _logger.LogInformation("Time entries synced. Created: {Created}.", createdTimeEntries);
            }
            catch (Exception ex)
            {
                timeEntriesOk = false;
                _logger.LogError(ex, "Failed to sync time entries.");
            }

            if (!projectsOk || !usersOk || !milestonesOk || !timeEntriesOk)
            {
                _logger.LogWarning("Some sync operations failed.");
            }

            _logger.LogInformation("Redmine sync job completed at {UtcNow}.", DateTime.UtcNow);
        }
    }
}
