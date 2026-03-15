using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.Commands;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Rules;
using Domain.Features.TimeEntries.Aggregates;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HumanResource.IntegrationTests.Features.Payrolls;

public class CreatePayrollIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task CreatePayroll_WhenValidData_ShouldGeneratePayrollAndUpdateBalances()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();

        var employee = new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 3000);
        var overtimeRule = new OvertimeRule(160, 1.5m, 20m);
        var deductionRule = new DeductionRule(0.08m, "ISR", DeductionType.BasicSalary);
        dbContext.BaseSalaryRules.Add(baseSalaryRule);
        dbContext.OvertimeRules.Add(overtimeRule);
        dbContext.DeductionRules.Add(deductionRule);
        await dbContext.SaveChangesAsync();

        for (int i = 0; i < 20; i++)
        {
            var entry = new TimeEntry(1000 + i, 101, employee.Id, 8, DateTime.UtcNow.AddDays(-i), 10, "Development");
            entry.Approve(8);
            dbContext.TimeEntries.Add(entry);
        }
        
        var extraEntry = new TimeEntry(2000, 101, employee.Id, 8, DateTime.UtcNow.AddDays(-1), 10, "Development");
        extraEntry.Approve(8);
        dbContext.TimeEntries.Add(extraEntry);
        await dbContext.SaveChangesAsync();

        var command = new CreatePayrollCommand
        {
            employeeId = employee.Id,
            periodStart = DateTime.UtcNow.AddDays(-30),
            periodEnd = DateTime.UtcNow
        };

        // Act
        var response = await Client.PostAsJsonAsync("/payrolls", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payrollResponse = await response.Content.ReadFromJsonAsync<Application.Features.Payrolls.Payroll.DTOs.PayrollResponse>();
        payrollResponse.Should().NotBeNull();
        payrollResponse!.EmployeeId.Should().Be(employee.Id);
        payrollResponse.Status.Should().Be("Calculated");

        payrollResponse.Components.Should().Contain(c => c.Type == "BaseSalary" && c.Amount == 3000);
        payrollResponse.Components.Should().Contain(c => c.Type == "Overtime");
        payrollResponse.Components.Should().Contain(c => c.Type == "LegalDeduction" && c.Category == "Deduction");

        payrollResponse.GrossAmount.Should().Be(3240);
        payrollResponse.TotalDeductions.Should().Be(240);
        payrollResponse.NetAmount.Should().Be(3000);

        var exists = await dbContext.Payrolls.AnyAsync(p => p.EmployeeId == employee.Id && p.PeriodStart == command.periodStart);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task CreatePayroll_WhenOverlappingPeriod_ShouldReturnBadRequest()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var employee = new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var payroll = new Domain.Features.Payrolls.Aggregates.Payroll(employee.Id, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
        dbContext.Payrolls.Add(payroll);
        await dbContext.SaveChangesAsync();

        var command = new CreatePayrollCommand
        {
            employeeId = employee.Id,
            periodStart = DateTime.UtcNow.AddDays(-30),
            periodEnd = DateTime.UtcNow
        };

        // Act
        var response = await Client.PostAsJsonAsync("/payrolls", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("overlaps with existing payroll");
    }
}
