using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.Features.Redmine.DTOs;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HumanResource.IntegrationTests.Features.Redmine;

public class SyncRedmineMilestonesIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task SyncMilestones_WhenNewMilestones_ShouldCreateAll()
    {
        // Arrange
        await ResetDatabaseAsync();
        var dbContext = await GetDbContextAsync();
        var project = new Project(123, "Project X");
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync();

        var redmineMilestones = new List<RedmineMilestoneDto>
        {
            new() { ProjectId = 123, Name = "Phase 1", Status = "open" },
            new() { ProjectId = 123, Name = "Phase 2", Status = "closed", CompletedAt = DateTime.UtcNow.AddDays(-1) }
        };
        RedmineServiceMock.Setup(x => x.GetAllProjectsAsync()).ReturnsAsync(new List<RedmineProjectDto> { new() { Id = 123 } });
        RedmineServiceMock.Setup(x => x.GetProjectMilestonesAsync(123)).ReturnsAsync(redmineMilestones);

        // Act
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineMilestonesHandler>();
        var created = await handler.Handle(CancellationToken.None);

        // Assert
        created.Should().Be(2);
        var milestones = await dbContext.ProjectMilestones.ToListAsync();
        milestones.Should().HaveCount(2);
        milestones.Should().Contain(m => m.Name == "Phase 1" && m.Status == MilestoneStatus.Pending);
        milestones.Should().Contain(m => m.Name == "Phase 2" && m.Status == MilestoneStatus.Completed);
    }

    [Fact]
    public async Task SyncMilestones_WhenExistingMilestoneChangesStatus_ShouldUpdate()
    {
        // Arrange
        await ResetDatabaseAsync();
        var dbContext = await GetDbContextAsync();
        var project = new Project(123, "Project X");
        dbContext.Projects.Add(project);
        
        var existing = new ProjectMilestone(123, "Phase 1");
        dbContext.ProjectMilestones.Add(existing);
        await dbContext.SaveChangesAsync();

        var initialStatus = await dbContext.ProjectMilestones.FirstAsync(m => m.Name == "Phase 1");
        initialStatus.Status.Should().Be(MilestoneStatus.Pending);

        var redmineMilestones = new List<RedmineMilestoneDto>
        {
            new() { ProjectId = 123, Name = "Phase 1", Status = "closed", CompletedAt = DateTime.UtcNow }
        };
        RedmineServiceMock.Setup(x => x.GetAllProjectsAsync()).ReturnsAsync(new List<RedmineProjectDto> { new() { Id = 123 } });
        RedmineServiceMock.Setup(x => x.GetProjectMilestonesAsync(123)).ReturnsAsync(redmineMilestones);

        // Act
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineMilestonesHandler>();
        var created = await handler.Handle(CancellationToken.None);

        // Assert
        created.Should().Be(0);
        var handlerDbContext = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.ApiDbContext>();
        var updated = await handlerDbContext.ProjectMilestones.FirstAsync(m => m.Name == "Phase 1");

        updated.Status.Should().Be(MilestoneStatus.Completed);
        updated.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }
}
