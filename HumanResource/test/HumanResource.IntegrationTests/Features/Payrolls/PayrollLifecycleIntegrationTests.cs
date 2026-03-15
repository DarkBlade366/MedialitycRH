using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.Commands;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Rules;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HumanResource.IntegrationTests.Features.Payrolls;

public class PayrollLifecycleIntegrationTests : IntegrationTestBase
{
    private async Task<Domain.Features.Payrolls.Aggregates.Payroll> SetupPayrollInCalculatedState()
    {
        var dbContext = await GetDbContextAsync();
        var employee = new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();

        var payroll = new Domain.Features.Payrolls.Aggregates.Payroll(employee.Id, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
        payroll.AddComponent(new PayrollComponent(PayrollComponentType.BaseSalary, PayrollComponentCategory.Earning, "Base", 3000, Guid.NewGuid()));
        payroll.MarkAsCalculated();
        dbContext.Payrolls.Add(payroll);
        await dbContext.SaveChangesAsync();
        return payroll;
    }

    [Fact]
    public async Task ApprovePayroll_WhenInCalculatedState_ShouldTransitionToApproved()
    {
        // Arrange
        var payroll = await SetupPayrollInCalculatedState();

        // Act
        var response = await Client.PostAsync($"/payrolls/approved/{payroll.Id}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dbContext = await GetDbContextAsync();
        var updated = await dbContext.Payrolls.FindAsync(payroll.Id);
        updated!.Status.Should().Be(PayrollStatus.Approved);
    }

    [Fact]
    public async Task ApprovePayroll_WhenAlreadyApproved_ShouldThrow()
    {
        // Arrange
        var payroll = await SetupPayrollInCalculatedState();
        payroll.Approve();
        var dbContext = await GetDbContextAsync();
        dbContext.Payrolls.Update(payroll);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.PostAsync($"/payrolls/approved/{payroll.Id}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task MarkAsPaid_WhenApproved_ShouldTransitionToPaid()
    {
        // Arrange
        var payroll = await SetupPayrollInCalculatedState();
        payroll.Approve();
        var dbContext = await GetDbContextAsync();
        dbContext.Payrolls.Update(payroll);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await Client.PostAsync($"/payrolls/paid/{payroll.Id}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var freshDbContext = await GetDbContextAsync();
        var updated = await freshDbContext.Payrolls.FindAsync(payroll.Id);
        updated!.Status.Should().Be(PayrollStatus.Paid);
    }

    [Fact]
    public async Task MarkAsPaid_WhenNotApproved_ShouldThrow()
    {
        // Arrange
        var payroll = await SetupPayrollInCalculatedState(); // still calculated

        // Act
        var response = await Client.PostAsync($"/payrolls/paid/{payroll.Id}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
