using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Redmine.DTOs;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Domain.Features.Projects.Aggregates;
using Domain.Features.TimeEntries.Aggregates;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Web.API.BackgroundServices;
using Xunit;

namespace HumanResource.IntegrationTests.BackgroundServices;

public class RedmineSyncBackgroundServiceIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task ExecuteAsync_WhenCalled_ShouldSyncAllComponents()
    {
        // Arrange
        var redmineUsers = new List<RedmineUserDto> { new RedmineUserDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@ex.com" } };
        var redmineProjects = new List<RedmineProjectDto> { new RedmineProjectDto { Id = 101, Name = "Project", Status = 1 } };
        var redmineMilestones = new List<RedmineMilestoneDto> { new RedmineMilestoneDto { ProjectId = 101, Name = "M1", Status = "closed", CompletedAt = DateTime.UtcNow } };
        var redmineTimeEntries = new List<RedmineTimeEntryDto>
        {
            new RedmineTimeEntryDto
            {
                Id = 1001,
                Hours = 8,
                SpentOn = DateTime.UtcNow.AddDays(-1),
                User = new RedmineUserReference { Id = 1 },
                Project = new RedmineProjectReference { Id = 101 },
                Activity = new RedmineActivityReference { Id = 10, Name = "Dev" }
            }
        };

        RedmineServiceMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(redmineUsers);
        RedmineServiceMock.Setup(x => x.GetProjectsAsync()).ReturnsAsync(redmineProjects);
        RedmineServiceMock.Setup(x => x.GetAllProjectsAsync()).ReturnsAsync(redmineProjects);
        RedmineServiceMock.Setup(x => x.GetProjectMilestonesAsync(101)).ReturnsAsync(redmineMilestones);
        RedmineServiceMock.Setup(x => x.GetTimeEntriesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1)).ReturnsAsync(redmineTimeEntries);

        var options = Options.Create(new RedmineSyncScheduleOptions { Enabled = true, IntervalHours = 24, InitialDelaySeconds = 1 });

        await ResetDatabaseAsync();
        var dbContext = await GetDbContextAsync();
        var employee = new Employee("John Doe", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        using var scope = Factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RedmineSyncBackgroundService>>();
        var job = scope.ServiceProvider.GetRequiredService<IRedmineSyncJob>();

        var service = new RedmineSyncBackgroundService(logger, job, options);

        await job.ExecuteAsync(CancellationToken.None);

        // Assert
        var assertDbContext = await GetDbContextAsync();
        var employees = await assertDbContext.Employees.ToListAsync();
        employees.Should().NotBeEmpty();

        var projects = await assertDbContext.Projects.ToListAsync();
        projects.Should().NotBeEmpty();

        var milestones = await assertDbContext.ProjectMilestones.ToListAsync();
        
        var entries = await assertDbContext.TimeEntries.ToListAsync();
        entries.Should().NotBeEmpty();
    }
}
