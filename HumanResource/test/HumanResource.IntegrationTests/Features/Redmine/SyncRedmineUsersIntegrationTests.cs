using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.Features.Redmine.DTOs;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HumanResource.IntegrationTests.Features.Redmine;

public class SyncRedmineUsersIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task SyncUsers_WhenNewUsers_ShouldCreateEmployees()
    {
        // Arrange
        await ResetDatabaseAsync();
        var redmineUsers = new List<RedmineUserDto>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
        };
        RedmineServiceMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(redmineUsers);

        // Act
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineUsersHandler>();
        var created = await handler.Handle(CancellationToken.None);
        var dbContext = await GetDbContextAsync();
        var employees = await dbContext.Employees.ToListAsync();
        employees.Should().HaveCount(2);
        employees.Should().Contain(e => e.FullName == "John Doe" && e.Email == "john@example.com" && e.RedmineUserId == 1);
        employees.Should().Contain(e => e.FullName == "Jane Smith" && e.Email == "jane@example.com" && e.RedmineUserId == 2);
    }

    [Fact]
    public async Task SyncUsers_WhenExistingUser_ShouldUpdateNameAndEmail()
    {
        // Arrange
        await ResetDatabaseAsync();
        var redmineUsers = new List<RedmineUserDto>
        {   
            new() { Id = 1, FirstName = "John", LastName = "Updated", Email = "john.updated@example.com" },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
        };
        RedmineServiceMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(redmineUsers);

        var dbContext = await GetDbContextAsync();
        
        dbContext.Employees.Add(new Employee("Old Name", "old@example.com", EmployeeRole.Employee, "hash", 1));
        await dbContext.SaveChangesAsync();
        
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineUsersHandler>();
        var created = await handler.Handle(CancellationToken.None);
        
        // Assert
        var handlerDbContext = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.ApiDbContext>();
        var updated = await handlerDbContext.Employees.FirstAsync(e => e.RedmineUserId == 1);
        updated.FullName.Should().Be("John Updated"); 
        updated.Email.Should().Be("john.updated@example.com"); 
    }

    [Fact]
    public async Task SyncUsers_WhenUserMissingInRedmine_ShouldDeactivateNonAdmin()
    {
        // Arrange
        await ResetDatabaseAsync();
        var redmineUsers = new List<RedmineUserDto>
        {
            new() { Id = 1, FirstName = "Active", LastName = "User", Email = "active@example.com" }
        };
        RedmineServiceMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(redmineUsers);

        var dbContext = await GetDbContextAsync();
        dbContext.Employees.Add(new Employee("Active User", "active@example.com", EmployeeRole.Employee, "hash", 1));
        dbContext.Employees.Add(new Employee("Admin User", "admin@example.com", EmployeeRole.Administrator, "hash", 2));
        await dbContext.SaveChangesAsync();
        
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineUsersHandler>();
        var created = await handler.Handle(CancellationToken.None);
        
        // Assert
        var handlerDbContext = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.ApiDbContext>();
        var allEmployees = await handlerDbContext.Employees.ToListAsync();
        
        var adminEmployee = allEmployees.FirstOrDefault(e => e.RedmineUserId == 2);
        adminEmployee.Should().NotBeNull();
        adminEmployee!.Role.Should().Be(EmployeeRole.Administrator);
        adminEmployee.IsActive.Should().BeTrue(); 
        
        var activeEmployee = allEmployees.FirstOrDefault(e => e.RedmineUserId == 1);
        activeEmployee.Should().NotBeNull();
        activeEmployee!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task SyncUsers_WhenNonAdminMissingInRedmine_ShouldDeactivate()
    {
        // Arrange
        await ResetDatabaseAsync();
        var redmineUsers = new List<RedmineUserDto>
        {
            new() { Id = 1, FirstName = "Active", LastName = "User", Email = "active@example.com" }
        };
        RedmineServiceMock.Setup(x => x.GetUsersAsync()).ReturnsAsync(redmineUsers);

        var dbContext = await GetDbContextAsync();
        dbContext.Employees.Add(new Employee("Active User", "active@example.com", EmployeeRole.Employee, "hash", 1));
        dbContext.Employees.Add(new Employee("Inactive User", "inactive@example.com", EmployeeRole.Employee, "hash", 2));
        dbContext.Employees.Add(new Employee("Admin User", "admin@example.com", EmployeeRole.Administrator, "hash", 3));
        await dbContext.SaveChangesAsync();
        
        using var scope = Factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<Application.Features.Redmine.Handlers.SyncRedmineUsersHandler>();
        var created = await handler.Handle(CancellationToken.None);
        
        // Assert
        var handlerDbContext = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.ApiDbContext>();
        var allEmployees = await handlerDbContext.Employees.ToListAsync();
        
        var activeEmployee = allEmployees.FirstOrDefault(e => e.RedmineUserId == 1);
        activeEmployee.Should().NotBeNull();
        activeEmployee!.IsActive.Should().BeTrue();
        
        var inactiveEmployee = allEmployees.FirstOrDefault(e => e.RedmineUserId == 2);
        inactiveEmployee.Should().NotBeNull();
        inactiveEmployee!.IsActive.Should().BeFalse();
        
        var adminEmployee = allEmployees.FirstOrDefault(e => e.RedmineUserId == 3);
        adminEmployee.Should().NotBeNull();
        adminEmployee!.Role.Should().Be(EmployeeRole.Administrator);
        adminEmployee.IsActive.Should().BeTrue(); 
    }
}
