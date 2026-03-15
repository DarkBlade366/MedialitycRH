using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.Features.Redmine.DTOs;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.TimeEntries.Aggregates;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HumanResource.IntegrationTests.Features.Redmine;

public class SyncRedmineTimeEntriesIntegrationTests : IntegrationTestBase
{
    private readonly DateTime _from = new(2024, 1, 1);
    private readonly DateTime _to = new(2024, 1, 31);

    [Fact]
    public async Task SyncTimeEntries_WhenNewEntries_ShouldCreateAll()
    {
        // Arrange
        await ResetDatabaseAsync();
        var dbContext = await GetDbContextAsync();
        var employee = new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hash", 100);
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        var redmineEntries = new List<RedmineTimeEntryDto>
        {
            new()
            {
                Id = 1001,
                Hours = 8,
                SpentOn = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                User = new RedmineUserReference { Id = 100 },
                Project = new RedmineProjectReference { Id = 1 },
                Activity = new RedmineActivityReference { Id = 10, Name = "Development" }
            }
        };
        RedmineServiceMock.Setup(x => x.GetTimeEntriesAsync(_from, _to, 100)).ReturnsAsync(redmineEntries);

        // Act
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineTimeEntriesHandler>();
        var created = await handler.Handle(_from, _to);

        // Assert
        created.Should().Be(1);
        var entries = await dbContext.TimeEntries.ToListAsync();
        entries.Should().ContainSingle(e =>
            e.RedmineTimeEntryId == 1001 &&
            e.EmployeeId == employee.Id &&
            e.Hours == 8 &&
            e.RedmineActivityId == 10 &&
            e.ActivityName == "Development");
    }

    [Fact]
    public async Task SyncTimeEntries_WhenEntryAlreadyExists_ShouldUpdate()
    {
        // Arrange
        await ResetDatabaseAsync();
        var dbContext = await GetDbContextAsync();
        var employee = new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hash", 100);
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        var existing = new TimeEntry(1001, 1, employee.Id, 7, new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc), 10, "Development");
        dbContext.TimeEntries.Add(existing);
        await dbContext.SaveChangesAsync();

        var redmineEntries = new List<RedmineTimeEntryDto>
        {
            new()
            {
                Id = 1001,
                Hours = 8.5m,
                SpentOn = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                User = new RedmineUserReference { Id = 100 },
                Project = new RedmineProjectReference { Id = 1 },
                Activity = new RedmineActivityReference { Id = 10, Name = "Development" }
            }
        };
        RedmineServiceMock.Setup(x => x.GetTimeEntriesAsync(_from, _to, 100)).ReturnsAsync(redmineEntries);

        // Act
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineTimeEntriesHandler>();
        var created = await handler.Handle(_from, _to);

        // Assert
        created.Should().Be(0);
        var handlerDbContext = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.ApiDbContext>();
        var updated = await handlerDbContext.TimeEntries.FirstAsync(e => e.RedmineTimeEntryId == 1001);
        updated.Hours.Should().Be(8.5m);
    }
}
