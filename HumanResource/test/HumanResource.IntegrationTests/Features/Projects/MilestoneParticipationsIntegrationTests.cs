using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Features.Projects.Commands;
using Application.Features.Projects.Queries;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Domain.Features.Projects.Aggregates;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HumanResource.IntegrationTests.Features.Projects;

public class MilestoneParticipationsIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateParticipation_WhenValid_ShouldAddParticipation()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var milestone = new ProjectMilestone(101, "Milestone 1");
        dbContext.ProjectMilestones.Add(milestone);
        await dbContext.SaveChangesAsync();

        var command = new CreateMilestoneParticipationCommand
        {
            ProjectMilestoneId = milestone.Id,
            EmployeeId = employee.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync("/milestone-participations", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var participationResponse = await response.Content.ReadFromJsonAsync<Application.Features.Projects.DTOs.MilestoneParticipationResponse>();
        participationResponse.Should().NotBeNull();
        participationResponse!.ProjectMilestoneId.Should().Be(milestone.Id);
        participationResponse.EmployeeId.Should().Be(employee.Id);
        participationResponse.IsPaid.Should().BeFalse();
        participationResponse.IsActive.Should().BeTrue();

        var participationInDb = await dbContext.Set<MilestoneParticipation>().FirstOrDefaultAsync(p => p.ProjectMilestoneId == milestone.Id && p.EmployeeId == employee.Id);
        participationInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task GetParticipationsPaged_ShouldReturnFilteredList()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var milestone = new ProjectMilestone(101, "Milestone 1");
        dbContext.ProjectMilestones.Add(milestone);
        var participation = new MilestoneParticipation(milestone.Id, employee.Id, milestone);
        dbContext.Set<MilestoneParticipation>().Add(participation);
        await dbContext.SaveChangesAsync();

        var query = new GetMilestoneParticipationsPagedQuery
        {
            Page = 1,
            PageSize = 10,
            ProjectMilestoneId = milestone.Id
        };

        // Act
        var response = await Client.GetAsync($"/milestone-participations?page=1&pageSize=10&ProjectMilestoneId={milestone.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paged = await response.Content.ReadFromJsonAsync<Application.Common.PagedResponse<Application.Features.Projects.DTOs.MilestoneParticipationResponse>>();
        paged!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task ChangeParticipationStatus_WhenNotPaid_ShouldToggleActive()
    {
        // Arrange
        var dbContext = await GetDbContextAsync();
        var employee = new Employee("John", "john@ex.com", EmployeeRole.Employee, "hash", 1);
        dbContext.Employees.Add(employee);
        var milestone = new ProjectMilestone(101, "Milestone 1");
        dbContext.ProjectMilestones.Add(milestone);
        await dbContext.SaveChangesAsync();

        var participation = new MilestoneParticipation(milestone.Id, employee.Id, milestone);
        dbContext.Set<MilestoneParticipation>().Add(participation);
        await dbContext.SaveChangesAsync();

        var command = new ChangeMilestoneParticipationStatusCommand
        {
            Id = participation.Id,
            IsActive = false
        };

        // Act
        var response = await Client.PutAsJsonAsync("/milestone-participations/status", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var freshDbContext = await GetDbContextAsync();
        var updated = await freshDbContext.Set<MilestoneParticipation>().FindAsync(participation.Id);
        updated!.IsActive.Should().BeFalse();
    }
}
