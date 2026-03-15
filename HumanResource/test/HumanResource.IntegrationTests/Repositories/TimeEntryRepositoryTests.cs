using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Domain.Features.TimeEntries.Aggregates;
using FluentAssertions;
using Infrastructure.Persistence.Repositories.TimeEntries;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HumanResource.IntegrationTests.Repositories;

public class TimeEntryRepositoryTests : IntegrationTestBase
{
    [Fact]
    public async Task GetWorkedHours_ShouldReturnSumOfApprovedHours()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new TimeEntryRepository(dbContext);
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var entries = new List<TimeEntry>
        {
            new(1001, 101, employee.Id, 8, DateTime.UtcNow.AddDays(-5), 10, "Dev"),
            new(1002, 101, employee.Id, 6, DateTime.UtcNow.AddDays(-4), 10, "Dev"),
            new(1003, 101, employee.Id, 8, DateTime.UtcNow.AddDays(-3), 10, "Dev")
        };
        entries[0].Approve(8);
        entries[1].Approve(5);
        entries[2].Approve(8);
        dbContext.TimeEntries.AddRange(entries);
        await dbContext.SaveChangesAsync();

        var from = DateTime.UtcNow.AddDays(-10);
        var to = DateTime.UtcNow;

        // Act
        var total = await repo.GetWorkedHours(employee.Id, from, to);

        // Assert
        total.Should().Be(21);
    }

    [Fact]
    public async Task GetByEmployeeAndPeriodAsync_ShouldReturnEntriesInRange()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new TimeEntryRepository(dbContext);
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var entryIn = new TimeEntry(1001, 101, employee.Id, 8, new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc), 10, "Dev");
        var entryOut = new TimeEntry(1002, 101, employee.Id, 8, new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc), 10, "Dev");
        dbContext.TimeEntries.AddRange(entryIn, entryOut);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repo.GetByEmployeeAndPeriodAsync(employee.Id, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc));

        // Assert
        result.Should().ContainSingle(e => e.Id == entryIn.Id);
        result.Should().NotContain(e => e.Id == entryOut.Id);
    }
}
