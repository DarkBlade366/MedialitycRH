using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using FluentAssertions;
using Infrastructure.Persistence.Repositories.Employees;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HumanResource.IntegrationTests.Repositories;

public class EmployeeRepositoryTests : IntegrationTestBase
{
    [Fact]
    public async Task AddAsync_ShouldPersistEmployee()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new EmployeeRepository(dbContext);
        var employee = new Employee("John Doe", "john@ex.com", EmployeeRole.Employee, "hash", 1);

        // Act
        await repo.AddAsync(employee);
        await dbContext.SaveChangesAsync();

        // Assert
        var fromDb = await dbContext.Employees
            .Include(e => e.AguinaldoBalance)
            .Include(e => e.VacationBalance)
            .FirstOrDefaultAsync(e => e.Id == employee.Id);
        fromDb.Should().NotBeNull();
        fromDb!.FullName.Should().Be("John Doe");
        fromDb.AguinaldoBalance.Should().NotBeNull();
        fromDb.VacationBalance.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnCorrectEmployee()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new EmployeeRepository(dbContext);
        var employee = new Employee("John Doe", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repo.GetByEmailAsync("john@ex.com");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(employee.Id);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        // Arrange
        await ResetDatabaseAsync();
        var dbContext = await GetDbContextAsync();
        var repo = new EmployeeRepository(dbContext);
        var employees = Enumerable.Range(1, 15).Select(i =>
            new Employee($"Name {i}", $"email{i}@ex.com", EmployeeRole.Employee, "hash", i)).ToList();
        dbContext.Employees.AddRange(employees);
        await dbContext.SaveChangesAsync();

        // Act
        var (items, total) = await repo.GetPagedAsync(2, 5);

        // Assert
        items.Should().HaveCount(5);
        total.Should().Be(15);
    }
}
