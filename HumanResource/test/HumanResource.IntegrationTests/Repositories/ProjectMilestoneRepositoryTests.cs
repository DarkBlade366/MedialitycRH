using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Enums;
using FluentAssertions;
using Infrastructure.Persistence.Repositories.Projects;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HumanResource.IntegrationTests.Repositories;

public class ProjectMilestoneRepositoryTests : IntegrationTestBase
{
    [Fact]
    public async Task AddRangeAsync_ShouldPersistMilestones()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new ProjectMilestoneRepository(dbContext);
        var milestones = new[]
        {
            new ProjectMilestone(101, "Milestone 1"),
            new ProjectMilestone(101, "Milestone 2")
        };

        // Act
        await repo.AddRangeAsync(milestones.ToList());
        await dbContext.SaveChangesAsync();

        // Assert
        var fromDb = await dbContext.ProjectMilestones.Where(m => m.RedmineProjectId == 101).ToListAsync();
        fromDb.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByProjectIdAsync_ShouldReturnMilestonesForProject()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new ProjectMilestoneRepository(dbContext);
        var milestone1 = new ProjectMilestone(101, "M1");
        var milestone2 = new ProjectMilestone(102, "M2");
        dbContext.ProjectMilestones.AddRange(milestone1, milestone2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repo.GetByProjectIdAsync(101);

        // Assert
        result.Should().ContainSingle(m => m.Name == "M1");
    }

    [Fact]
    public async Task GetByProjectAndNameAsync_ShouldReturnMilestone()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new ProjectMilestoneRepository(dbContext);
        var milestone = new ProjectMilestone(101, "Unique");
        dbContext.ProjectMilestones.Add(milestone);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repo.GetByProjectAndNameAsync(101, "Unique");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(milestone.Id);
    }

    [Fact]
    public async Task GetCompletedAsync_ShouldReturnOnlyCompletedMilestones()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new ProjectMilestoneRepository(dbContext);
        var pending = new ProjectMilestone(101, "Pending");
        var completed = new ProjectMilestone(101, "Completed");
        completed.MarkAsCompleted(DateTime.UtcNow);
        dbContext.ProjectMilestones.AddRange(pending, completed);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repo.GetCompletedAsync();

        // Assert
        result.Should().ContainSingle(m => m.Name == "Completed");
        result.Should().NotContain(m => m.Name == "Pending");
    }
}
