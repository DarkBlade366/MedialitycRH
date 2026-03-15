using System;
using System.Threading.Tasks;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Domain.Features.Projects.Aggregates;
using FluentAssertions;
using Infrastructure.Persistence.Repositories.Projects;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HumanResource.IntegrationTests.Repositories;

public class MilestoneParticipationRepositoryTests : IntegrationTestBase
{
    [Fact]
    public async Task AddAsync_ShouldPersistParticipation()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new MilestoneParticipationRepository(dbContext);
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var milestone = new ProjectMilestone(101, "M1");
        dbContext.ProjectMilestones.Add(milestone);
        await dbContext.SaveChangesAsync();

        var participation = new MilestoneParticipation(milestone.Id, employee.Id, milestone);

        // Act
        await repo.AddAsync(participation);
        await dbContext.SaveChangesAsync();

        // Assert
        var fromDb = await dbContext.Set<MilestoneParticipation>()
            .FirstOrDefaultAsync(p => p.ProjectMilestoneId == milestone.Id && p.EmployeeId == employee.Id);
        fromDb.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByEmployeeIdAsync_ShouldReturnParticipationsForEmployee()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new MilestoneParticipationRepository(dbContext);
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var milestone1 = new ProjectMilestone(101, "M1");
        var milestone2 = new ProjectMilestone(102, "M2");
        dbContext.ProjectMilestones.AddRange(milestone1, milestone2);
        await dbContext.SaveChangesAsync();

        var participation1 = new MilestoneParticipation(milestone1.Id, employee.Id, milestone1);
        var participation2 = new MilestoneParticipation(milestone2.Id, employee.Id, milestone2);
        dbContext.Set<MilestoneParticipation>().AddRange(participation1, participation2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repo.GetByEmployeeIdAsync(employee.Id);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrueIfExists()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new MilestoneParticipationRepository(dbContext);
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var milestone = new ProjectMilestone(101, "M1");
        dbContext.ProjectMilestones.Add(milestone);
        await dbContext.SaveChangesAsync();

        var participation = new MilestoneParticipation(milestone.Id, employee.Id, milestone);
        dbContext.Set<MilestoneParticipation>().Add(participation);
        await dbContext.SaveChangesAsync();

        // Act
        var exists = await repo.ExistsAsync(milestone.Id, employee.Id);

        // Assert
        exists.Should().BeTrue();
    }
}
