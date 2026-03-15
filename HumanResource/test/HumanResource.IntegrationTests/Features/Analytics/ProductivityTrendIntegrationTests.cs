using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using FluentAssertions;
using Domain.Features.Employees.Interfaces;
using Domain.Features.TimeEntries.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.TimeEntries.Aggregates;
using Domain.Features.Payrolls.Entities;
using Application.Features.Analytics.DTOs;
using Application.Features.Analytics.Queries;
using Application.Features.Analytics.Handlers;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Domain.Features.Employees.Aggregates;
using Moq;

namespace HumanResource.IntegrationTests.Features.Analytics;

public class ProductivityTrendIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_WithValidEmployee_ShouldReturnProductivityTrend()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        
        var employee = new Employee(
            "John Doe",
            "john@example.com",
            Domain.Features.Employees.Enums.EmployeeRole.Employee,
            "hashedpassword",
            123);

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        var weights = new List<ActivityProductivityWeight>
        {
            new ActivityProductivityWeight(1, "Development", 0.8m),
            new ActivityProductivityWeight(2, "Testing", 1.0m)
        };

        foreach (var weight in weights)
        {
            dbContext.ActivityProductivityWeights.Add(weight);
        }
        await dbContext.SaveChangesAsync();

        var timeEntries = new List<TimeEntry>
        {
            new TimeEntry(1, 1, employee.Id, 40m, DateTime.UtcNow.AddMonths(-2)),
            new TimeEntry(2, 2, employee.Id, 30m, DateTime.UtcNow.AddMonths(-1))
        };

        foreach (var entry in timeEntries)
        {
            entry.Approve(entry.Hours);
            dbContext.TimeEntries.Add(entry);
        }
        await dbContext.SaveChangesAsync();

        using var scope = Factory.Services.CreateScope();
        var handler = new GetProductivityTrendHandler(
            scope.ServiceProvider.GetRequiredService<ITimeEntryRepository>(),
            scope.ServiceProvider.GetRequiredService<IActivityProductivityWeightRepository>(),
            scope.ServiceProvider.GetRequiredService<IEmployeeRepository>());

        var query = new GetProductivityTrendQuery
        {
            EmployeeId = employee.Id,
            From = DateTime.UtcNow.AddMonths(-3),
            To = DateTime.UtcNow
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().BeGreaterThanOrEqualTo(2);
        
        var monthWithData1 = result.FirstOrDefault(r => r.Year == DateTime.UtcNow.AddMonths(-2).Year && r.Month == DateTime.UtcNow.AddMonths(-2).Month);
        monthWithData1.Should().NotBeNull();
        monthWithData1.Metric.Should().Be(40m);

        var monthWithData2 = result.FirstOrDefault(r => r.Year == DateTime.UtcNow.AddMonths(-1).Year && r.Month == DateTime.UtcNow.AddMonths(-1).Month);
        monthWithData2.Should().NotBeNull();
        monthWithData2.Metric.Should().Be(30m);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentEmployee_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var handler = new GetProductivityTrendHandler(
            scope.ServiceProvider.GetRequiredService<ITimeEntryRepository>(),
            scope.ServiceProvider.GetRequiredService<IActivityProductivityWeightRepository>(),
            scope.ServiceProvider.GetRequiredService<IEmployeeRepository>());

        var query = new GetProductivityTrendQuery
        {
            EmployeeId = Guid.NewGuid(),
            From = DateTime.UtcNow.AddMonths(-1),
            To = DateTime.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_WithNoTimeEntries_ShouldReturnEmptyList()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        
        var employee = new Employee(
            "Jane Doe",
            "jane@example.com",
            Domain.Features.Employees.Enums.EmployeeRole.Employee,
            "hashedpassword",
            456);

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        using var scope = Factory.Services.CreateScope();
        var handler = new GetProductivityTrendHandler(
            scope.ServiceProvider.GetRequiredService<ITimeEntryRepository>(),
            scope.ServiceProvider.GetRequiredService<IActivityProductivityWeightRepository>(),
            scope.ServiceProvider.GetRequiredService<IEmployeeRepository>());

        var query = new GetProductivityTrendQuery
        {
            EmployeeId = employee.Id,
            From = DateTime.UtcNow.AddMonths(-1),
            To = DateTime.UtcNow
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        
        result.Should().OnlyContain(r => r.Metric == 0m);
    }
}
