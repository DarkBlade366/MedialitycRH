using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Enums;
using FluentAssertions;
using Infrastructure.Persistence.Repositories.Payrrolls;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HumanResource.IntegrationTests.Repositories;

public class PayrollRepositoryTests : IntegrationTestBase
{
    [Fact]
    public async Task AddAsync_ShouldPersistPayrollWithComponents()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new PayrollRepository(dbContext);
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        var payroll = new Payroll(employee.Id, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
        payroll.AddComponent(new PayrollComponent(PayrollComponentType.BaseSalary, PayrollComponentCategory.Earning, "Base", 3000, Guid.NewGuid()));
        payroll.MarkAsCalculated();

        // Act
        await repo.AddAsync(payroll);
        await dbContext.SaveChangesAsync();

        // Assert
        var fromDb = await repo.GetByIdAsync(payroll.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Components.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByEmployeeAndPeriodAsync_ShouldReturnCorrectPayroll()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new PayrollRepository(dbContext);
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var periodStart = DateTime.UtcNow.AddDays(-30);
        var periodEnd = DateTime.UtcNow;
        var payroll = new Payroll(employee.Id, DateTime.SpecifyKind(periodStart, DateTimeKind.Utc), DateTime.SpecifyKind(periodEnd, DateTimeKind.Utc));
        dbContext.Payrolls.Add(payroll);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repo.GetByEmployeeAndPeriodAsync(employee.Id, DateTime.SpecifyKind(periodStart, DateTimeKind.Utc), DateTime.SpecifyKind(periodEnd, DateTimeKind.Utc));

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(payroll.Id);
    }

    [Fact]
    public async Task ExistsOverlappingPayroll_ShouldDetectOverlap()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var repo = new PayrollRepository(dbContext);
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var payroll = new Payroll(employee.Id, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 31, 0, 0, 0, DateTimeKind.Utc));
        dbContext.Payrolls.Add(payroll);
        await dbContext.SaveChangesAsync();

        // Act
        var overlap = await repo.ExistsOverlappingPayroll(employee.Id, new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc));
        var noOverlap = await repo.ExistsOverlappingPayroll(employee.Id, new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 2, 28, 0, 0, 0, DateTimeKind.Utc));

        // Assert
        overlap.Should().BeTrue();
        noOverlap.Should().BeFalse();
    }
}
