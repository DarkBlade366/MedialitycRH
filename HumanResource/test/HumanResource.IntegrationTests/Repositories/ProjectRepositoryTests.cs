using System.Threading.Tasks;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Enums;
using FluentAssertions;
using Infrastructure.Persistence.Repositories.Projects;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HumanResource.IntegrationTests.Repositories;

public class ProjectRepositoryTests : IntegrationTestBase
{
    [Fact]
    public async Task AddAsync_ShouldPersistProject()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new ProjectRepository(dbContext);
        var project = new Project(101, "Test Project", ProjectStatus.Active);

        // Act
        await repo.AddAsync(project);
        await dbContext.SaveChangesAsync();

        // Assert
        var fromDb = await dbContext.Projects.FirstOrDefaultAsync(p => p.RedmineProjectId == 101);
        fromDb.Should().NotBeNull();
        fromDb!.Name.Should().Be("Test Project");
    }

    [Fact]
    public async Task GetByRedmineIdAsync_ShouldReturnProject()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new ProjectRepository(dbContext);
        var project = new Project(101, "Test Project", ProjectStatus.Active);
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repo.GetByRedmineIdAsync(101);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(project.Id);
    }

    [Fact]
    public async Task Update_ShouldModifyProject()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new ProjectRepository(dbContext);
        var project = new Project(101, "Old Name", ProjectStatus.Active);
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync();

        // Act
        project.UpdateName("New Name");
        repo.Update(project);
        await dbContext.SaveChangesAsync();

        // Assert
        var updated = await dbContext.Projects.FirstAsync(p => p.RedmineProjectId == 101);
        updated.Name.Should().Be("New Name");
    }
}
