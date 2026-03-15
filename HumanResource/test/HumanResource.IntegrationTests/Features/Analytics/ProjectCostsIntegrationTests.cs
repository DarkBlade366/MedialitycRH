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
using Domain.Features.Payrolls.Rules;
using Application.Features.Analytics.DTOs;
using Application.Features.Analytics.Queries;
using Application.Features.Analytics.Handlers;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Domain.Features.Employees.Aggregates;
using Moq;

namespace HumanResource.IntegrationTests.Features.Analytics;

public class ProjectCostsIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_WithValidData_ShouldReturnProjectCosts()
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

        var salaryRule = new BaseSalaryRule(Domain.Features.Employees.Enums.EmployeeRole.Employee, 3000m);
        dbContext.BaseSalaryRules.Add(salaryRule);
        await dbContext.SaveChangesAsync();

        var timeEntries = new List<TimeEntry>
        {
            new TimeEntry(1, 1, employee.Id, 40m, DateTime.UtcNow.AddDays(-10)),
            new TimeEntry(2, 2, employee.Id, 20m, DateTime.UtcNow.AddDays(-5))
        };

        foreach (var entry in timeEntries)
        {
            entry.Approve(entry.Hours);
            dbContext.TimeEntries.Add(entry);
        }
        await dbContext.SaveChangesAsync();

        using var scope = Factory.Services.CreateScope();
        var handler = new GetProjectCostsHandler(
            scope.ServiceProvider.GetRequiredService<ITimeEntryRepository>(),
            scope.ServiceProvider.GetRequiredService<IEmployeeRepository>(),
            scope.ServiceProvider.GetRequiredService<IBaseSalaryRuleRepository>());

        var query = new GetProjectCostsQuery
        {
            PeriodStart = DateTime.UtcNow.AddDays(-15),
            PeriodEnd = DateTime.UtcNow
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var project1 = result.FirstOrDefault(r => r.RedmineProjectId == 1);
        project1.Should().NotBeNull();
        project1.TotalHours.Should().Be(40m);
        project1.EstimatedCost.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task HandleAsync_WithNoApprovedEntries_ShouldReturnEmptyList()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        
        var employee = new Employee(
            "Bob Smith",
            "bob@example.com",
            Domain.Features.Employees.Enums.EmployeeRole.Employee,
            "hashedpassword",
            789);

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        var timeEntry = new TimeEntry(1, 1, employee.Id, 25m, DateTime.UtcNow.AddDays(-5));
        dbContext.TimeEntries.Add(timeEntry);
        await dbContext.SaveChangesAsync();

        using var scope = Factory.Services.CreateScope();
        var handler = new GetProjectCostsHandler(
            scope.ServiceProvider.GetRequiredService<ITimeEntryRepository>(),
            scope.ServiceProvider.GetRequiredService<IEmployeeRepository>(),
            scope.ServiceProvider.GetRequiredService<IBaseSalaryRuleRepository>());

        var query = new GetProjectCostsQuery
        {
            PeriodStart = DateTime.UtcNow.AddDays(-10),
            PeriodEnd = DateTime.UtcNow
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
