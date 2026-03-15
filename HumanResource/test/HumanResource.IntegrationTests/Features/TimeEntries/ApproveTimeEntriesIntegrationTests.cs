using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Features.TimeEntries.Commands;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Domain.Features.TimeEntries.Aggregates;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HumanResource.IntegrationTests.Features.TimeEntries;

public class ApproveTimeEntriesIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task ApproveTimeEntry_WhenValid_ShouldSetApprovedHoursAndReviewed()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var entry = new TimeEntry(1001, 101, employee.Id, 8, DateTime.UtcNow.AddDays(-1), 10, "Development");
        dbContext.TimeEntries.Add(entry);
        await dbContext.SaveChangesAsync();

        var command = new ApproveTimeEntryCommand
        {
            TimeEntryId = entry.Id,
            ApprovedHours = 7.5m
        };

        // Act
        var response = await Client.PutAsJsonAsync("/time-entries/approve", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await GetDbContextAsync();
        var updatedEntry = await updated.TimeEntries.FindAsync(entry.Id);
        updatedEntry!.ApprovedHours.Should().Be(7.5m);
        updatedEntry.Reviewed.Should().BeTrue();
    }

    [Fact]
    public async Task ApproveTimeEntriesBatch_WhenMultipleEntries_ShouldProcessAll()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var entry1 = new TimeEntry(1001, 101, employee.Id, 8, DateTime.UtcNow.AddDays(-1), 10, "Dev");
        var entry2 = new TimeEntry(1002, 101, employee.Id, 6, DateTime.UtcNow.AddDays(-2), 10, "Dev");
        dbContext.TimeEntries.AddRange(entry1, entry2);
        await dbContext.SaveChangesAsync();

        var batch = new ApproveTimeEntriesBatchCommand
        {
            Items = new List<ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem>
            {
                new() { TimeEntryId = entry1.Id, ApprovedHours = 8 },
                new() { TimeEntryId = entry2.Id, ApprovedHours = 5 }
            }
        };

        // Act
        var response = await Client.PutAsJsonAsync("/time-entries/approve-batch", batch);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<Application.Features.TimeEntries.DTOs.TimeEntryBatchResultDto>>();
        results.Should().HaveCount(2);
        results.Should().AllSatisfy(r => r.Success.Should().BeTrue());

        var updated = await GetDbContextAsync();
        var updated1 = await updated.TimeEntries.FindAsync(entry1.Id);
        updated1!.ApprovedHours.Should().Be(8);
        var updated2 = await updated.TimeEntries.FindAsync(entry2.Id);
        updated2!.ApprovedHours.Should().Be(5);
    }

    [Fact]
    public async Task ApproveTimeEntry_WhenAlreadyReviewed_ShouldFail()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var entry = new TimeEntry(1001, 101, employee.Id, 8, DateTime.UtcNow.AddDays(-1), 10, "Dev");
        entry.Approve(8);
        dbContext.TimeEntries.Add(entry);
        await dbContext.SaveChangesAsync();

        var command = new ApproveTimeEntryCommand { TimeEntryId = entry.Id, ApprovedHours = 7 };

        // Act
        var response = await Client.PutAsJsonAsync("/time-entries/approve", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
