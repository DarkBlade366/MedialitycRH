using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Features.Redmine.DTOs;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Enums;
using Domain.Features.TimeEntries.Aggregates;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HumanResource.IntegrationTests.Features.Redmine;

public class FullSyncWorkflowTests : IntegrationTestBase
{
    [Fact]
    public async Task FullSync_AllSteps_ShouldSynchronizeAllData()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();

        var redmineUsers = new List<RedmineUserDto>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" }
        };
        RedmineServiceMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(redmineUsers);

        var redmineProjects = new List<RedmineProjectDto>
        {
            new() { Id = 101, Name = "Project Alpha", Status = 1 }
        };
        RedmineServiceMock.Setup(x => x.GetProjectsAsync()).ReturnsAsync(redmineProjects);
        RedmineServiceMock.Setup(x => x.GetAllProjectsAsync()).ReturnsAsync(redmineProjects);

        var redmineMilestones = new List<RedmineMilestoneDto>
        {
            new() { ProjectId = 101, Name = "Milestone 1", Status = "closed", CompletedAt = DateTime.UtcNow.AddDays(-1) }
        };
        RedmineServiceMock.Setup(x => x.GetProjectMilestonesAsync(101)).ReturnsAsync(redmineMilestones);

        var redmineEntries = new List<RedmineTimeEntryDto>
        {
            new()
            {
                Id = 1001,
                Hours = 8,
                SpentOn = DateTime.UtcNow.AddDays(-2),
                User = new RedmineUserReference { Id = 1 },
                Project = new RedmineProjectReference { Id = 101 },
                Activity = new RedmineActivityReference { Id = 10, Name = "Development" }
            }
        };
        RedmineServiceMock.Setup(x => x.GetTimeEntriesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1)).ReturnsAsync(redmineEntries);

        // Act: ejecutar sincronizaciones en orden
        using var scope = Factory.Services.CreateScope();
        var usersHandler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineUsersHandler>();
        var projectsHandler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineProjectsHandler>();
        var milestonesHandler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineMilestonesHandler>();
        var timeEntriesHandler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineTimeEntriesHandler>();

        var createdUsers = await usersHandler.Handle(CancellationToken.None);
        var createdProjects = await projectsHandler.Handle(CancellationToken.None);
        var createdMilestones = await milestonesHandler.Handle(CancellationToken.None);
        var createdTimeEntries = await timeEntriesHandler.Handle(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);

        // Assert
        createdUsers.Should().Be(1);
        createdProjects.Should().Be(1);
        createdMilestones.Should().Be(1);
        createdTimeEntries.Should().Be(1);

        var employee = await dbContext.Employees.FirstAsync(e => e.RedmineUserId == 1);
        employee.Should().NotBeNull();

        var project = await dbContext.Projects.FirstAsync(p => p.RedmineProjectId == 101);
        project.Should().NotBeNull();

        var milestone = await dbContext.ProjectMilestones.FirstAsync(m => m.RedmineProjectId == 101 && m.Name == "Milestone 1");
        milestone.Status.Should().Be(MilestoneStatus.Completed);

        var timeEntry = await dbContext.TimeEntries.FirstAsync(t => t.RedmineTimeEntryId == 1001);
        timeEntry.EmployeeId.Should().Be(employee.Id);
        timeEntry.RedmineProjectId.Should().Be(101);
    }
}
