using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Infrastructure.Redmine;
using Application.Features.Redmine.DTOs;
using FluentAssertions;

namespace Infrastructure.Redmine
{
    public class RedmineClientTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<RedmineClient>> _loggerMock;
        private readonly RedmineClient _redmineClient;

        public RedmineClientTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _httpClient.BaseAddress = new Uri("https://test.redmine.com");
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<RedmineClient>>();
            _redmineClient = new RedmineClient(_httpClient, _configurationMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUsersAsync_WhenApiResponseIsValid_ShouldReturnUsers()
        {
            // Arrange
            var expectedUsers = new List<RedmineUserDto>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new() { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
            };

            var response = new RedmineUsersResponse { Users = expectedUsers };
            var jsonContent = JsonSerializer.Serialize(response);

            SetupHttpResponse("/users.json", jsonContent);

            // Act
            var result = await _redmineClient.GetUsersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            var users = result!;
            users[0].FirstName.Should().Be("John");
            users[0].LastName.Should().Be("Doe");
            users[0].Email.Should().Be("john@example.com");
            users[1].FirstName.Should().Be("Jane");
            users[1].LastName.Should().Be("Smith");
            users[1].Email.Should().Be("jane@example.com");
        }

        [Fact]
        public async Task GetUsersAsync_WhenApiResponseIsEmpty_ShouldReturnEmptyList()
        {
            // Arrange
            var response = new RedmineUsersResponse { Users = new List<RedmineUserDto>() };
            var jsonContent = JsonSerializer.Serialize(response);

            SetupHttpResponse("/users.json", jsonContent);

            // Act
            var result = await _redmineClient.GetUsersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUsersAsync_WhenApiReturns404_ShouldThrowHttpRequestException()
        {
            // Arrange
            SetupHttpErrorResponse("/users.json", HttpStatusCode.NotFound);

            // Act
            Func<Task> act = async () => await _redmineClient.GetUsersAsync();

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.Message.Contains("404"));
        }

        [Fact]
        public async Task GetUsersAsync_WhenApiReturns500_ShouldThrowHttpRequestException()
        {
            // Arrange
            SetupHttpErrorResponse("/users.json", HttpStatusCode.InternalServerError);

            // Act
            Func<Task> act = async () => await _redmineClient.GetUsersAsync();

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.Message.Contains("500"));
        }

        [Fact]
        public async Task GetProjectsAsync_WhenAllStatusesReturnValidData_ShouldReturnAllUniqueProjects()
        {
            // Arrange
            var activeProjects = new List<RedmineProjectDto>
            {
                new() { Id = 1, Name = "Active Project 1", Status = 1 },
                new() { Id = 2, Name = "Active Project 2", Status = 1 }
            };

            var closedProjects = new List<RedmineProjectDto>
            {
                new() { Id = 3, Name = "Closed Project 1", Status = 5 },
                new() { Id = 1, Name = "Active Project 1", Status = 1 }
            };

            var archivedProjects = new List<RedmineProjectDto>
            {
                new() { Id = 4, Name = "Archived Project 1", Status = 9 }
            };

            SetupHttpResponse("/projects.json?status=1", JsonSerializer.Serialize(new RedmineProjectsResponse { Projects = activeProjects }));
            SetupHttpResponse("/projects.json?status=5", JsonSerializer.Serialize(new RedmineProjectsResponse { Projects = closedProjects }));
            SetupHttpResponse("/projects.json?status=9", JsonSerializer.Serialize(new RedmineProjectsResponse { Projects = archivedProjects }));

            // Act
            var result = await _redmineClient.GetProjectsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(4);
            result.Should().Contain(p => p.Id == 1 && p.Name == "Active Project 1");
            result.Should().Contain(p => p.Id == 2 && p.Name == "Active Project 2");
            result.Should().Contain(p => p.Id == 3 && p.Name == "Closed Project 1");
            result.Should().Contain(p => p.Id == 4 && p.Name == "Archived Project 1");
        }

        [Fact]
        public async Task GetProjectsAsync_WhenFirstCallFails_ShouldUseFallbackEndpoint()
        {
            // Arrange
            var fallbackProjects = new List<RedmineProjectDto>
            {
                new() { Id = 1, Name = "Fallback Project", Status = 1 }
            };

            SetupHttpErrorResponse("/projects.json?status=1", HttpStatusCode.InternalServerError);
            SetupHttpResponse("/projects.json", JsonSerializer.Serialize(new RedmineProjectsResponse { Projects = fallbackProjects }));

            // Act
            var result = await _redmineClient.GetProjectsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainSingle();
            var project = result!.Single();
            project.Name.Should().Be("Fallback Project");
        }

        [Fact]
        public async Task GetProjectsAsync_WhenAllEndpointsFail_ShouldThrowException()
        {
            // Arrange
            SetupHttpErrorResponse("/projects.json?status=1", HttpStatusCode.InternalServerError);
            SetupHttpErrorResponse("/projects.json?status=5", HttpStatusCode.InternalServerError);
            SetupHttpErrorResponse("/projects.json?status=9", HttpStatusCode.InternalServerError);
            SetupHttpErrorResponse("/projects.json", HttpStatusCode.InternalServerError);

            // Act
            Func<Task> act = async () => await _redmineClient.GetProjectsAsync();

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>();
        }

        [Fact]
        public async Task GetAllProjectsAsync_WhenApiResponseIsValid_ShouldReturnProjects()
        {
            // Arrange
            var expectedProjects = new List<RedmineProjectDto>
            {
                new() { Id = 1, Name = "Project 1", Status = 1 },
                new() { Id = 2, Name = "Project 2", Status = 5 }
            };

            var response = new RedmineProjectsResponse { Projects = expectedProjects };
            var jsonContent = JsonSerializer.Serialize(response);

            SetupHttpResponse("/projects.json", jsonContent);

            // Act
            var result = await _redmineClient.GetAllProjectsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            var projects = result!;
            projects[0].Name.Should().Be("Project 1");
            projects[0].Status.Should().Be(1);
            projects[1].Name.Should().Be("Project 2");
            projects[1].Status.Should().Be(5);
        }

        [Fact]
        public async Task GetAllProjectsAsync_WhenApiReturns404_ShouldThrowHttpRequestException()
        {
            // Arrange
            SetupHttpErrorResponse("/projects.json", HttpStatusCode.NotFound);

            // Act
            Func<Task> act = async () => await _redmineClient.GetAllProjectsAsync();

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.Message.Contains("404"));
        }

        [Fact]
        public async Task GetTimeEntriesAsync_WithValidParameters_ShouldReturnTimeEntries()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 31);
            var expectedTimeEntries = new List<RedmineTimeEntryDto>
            {
                new()
                {
                    Id = 1,
                    Hours = 8m,
                    SpentOn = new DateTime(2024, 1, 15),
                    User = new RedmineUserReference { Id = 123 },
                    Project = new RedmineProjectReference { Id = 456 },
                    Activity = new RedmineActivityReference { Id = 10, Name = "Development" }
                }
            };

            var response = new RedmineTimeEntriesResponse { TimeEntries = expectedTimeEntries };
            var jsonContent = JsonSerializer.Serialize(response);

            SetupHttpResponse($"/time_entries.json?from={fromDate:yyyy-MM-dd}&to={toDate:yyyy-MM-dd}", jsonContent);

            // Act
            var result = await _redmineClient.GetTimeEntriesAsync(fromDate, toDate);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainSingle();
            var entry = result!.Single();
            entry.Id.Should().Be(1);
            entry.Hours.Should().Be(8m);
            entry.SpentOn.Should().Be(new DateTime(2024, 1, 15));
            entry.User.Id.Should().Be(123);
            entry.Project.Id.Should().Be(456);
            entry.Activity.Id.Should().Be(10);
            entry.Activity.Name.Should().Be("Development");
        }

        [Fact]
        public async Task GetTimeEntriesAsync_WithUserId_ShouldIncludeUserIdInUrl()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 31);
            var userId = 123;
            var expectedTimeEntries = new List<RedmineTimeEntryDto>
            {
                new()
                {
                    Id = 1,
                    Hours = 6m,
                    SpentOn = new DateTime(2024, 1, 10),
                    User = new RedmineUserReference { Id = userId }
                }
            };

            var response = new RedmineTimeEntriesResponse { TimeEntries = expectedTimeEntries };
            var jsonContent = JsonSerializer.Serialize(response);

            SetupHttpResponse($"/time_entries.json?from={fromDate:yyyy-MM-dd}&to={toDate:yyyy-MM-dd}&user_id={userId}", jsonContent);

            // Act
            var result = await _redmineClient.GetTimeEntriesAsync(fromDate, toDate, userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainSingle();
            var entry = result!.Single();
            entry.User.Id.Should().Be(userId);
        }

        [Fact]
        public async Task GetTimeEntriesAsync_WhenApiReturns404_ShouldThrowHttpRequestException()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 31);
            SetupHttpErrorResponse($"/time_entries.json?from={fromDate:yyyy-MM-dd}&to={toDate:yyyy-MM-dd}", HttpStatusCode.NotFound);

            // Act
            Func<Task> act = async () => await _redmineClient.GetTimeEntriesAsync(fromDate, toDate);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.Message.Contains("404"));
        }

        [Fact]
        public async Task GetProjectMilestonesAsync_WithValidProjectId_ShouldReturnMilestones()
        {
            // Arrange
            var projectId = 123;
            var expectedMilestones = new List<RedmineMilestoneDto>
            {
                new() { ProjectId = projectId, Name = "Milestone 1", Status = "open", CompletedAt = null },
                new() { ProjectId = projectId, Name = "Milestone 2", Status = "closed", CompletedAt = new DateTime(2024, 1, 15) }
            };

            var response = new RedmineMilestonesResponse { Milestones = expectedMilestones };
            var jsonContent = JsonSerializer.Serialize(response);

            SetupHttpResponse($"/projects/{projectId}/versions.json", jsonContent);

            // Act
            var result = await _redmineClient.GetProjectMilestonesAsync(projectId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            var milestones = result!;
            milestones[0].ProjectId.Should().Be(projectId);
            milestones[0].Name.Should().Be("Milestone 1");
            milestones[0].Status.Should().Be("open");
            milestones[0].CompletedAt.Should().BeNull();

            milestones[1].ProjectId.Should().Be(projectId);
            milestones[1].Name.Should().Be("Milestone 2");
            milestones[1].Status.Should().Be("closed");
            milestones[1].CompletedAt.Should().Be(new DateTime(2024, 1, 15));
        }

        [Fact]
        public async Task GetProjectMilestonesAsync_WhenApiReturns404_ShouldThrowHttpRequestException()
        {
            // Arrange
            var projectId = 999;
            SetupHttpErrorResponse($"/projects/{projectId}/versions.json", HttpStatusCode.NotFound);

            // Act
            Func<Task> act = async () => await _redmineClient.GetProjectMilestonesAsync(projectId);

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.Message.Contains("404"));
        }

        [Fact]
        public async Task GetTimeEntryActivitiesAsync_WhenApiResponseIsValid_ShouldReturnActivities()
        {
            // Arrange
            var expectedActivities = new List<RedmineTimeEntryActivityDto>
            {
                new() { Id = 10, Name = "Development" },
                new() { Id = 20, Name = "Testing" },
                new() { Id = 30, Name = "Documentation" }
            };

            var response = new RedmineTimeEntryActivitiesResponse { TimeEntryActivities = expectedActivities };
            var jsonContent = JsonSerializer.Serialize(response);

            SetupHttpResponse("/enumerations/time_entry_activities.json", jsonContent);

            // Act
            var result = await _redmineClient.GetTimeEntryActivitiesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            var activities = result!;
            activities[0].Id.Should().Be(10);
            activities[0].Name.Should().Be("Development");
            activities[1].Id.Should().Be(20);
            activities[1].Name.Should().Be("Testing");
            activities[2].Id.Should().Be(30);
            activities[2].Name.Should().Be("Documentation");
        }

        [Fact]
        public async Task GetTimeEntryActivitiesAsync_WhenApiReturns404_ShouldThrowHttpRequestException()
        {
            // Arrange
            SetupHttpErrorResponse("/enumerations/time_entry_activities.json", HttpStatusCode.NotFound);

            // Act
            Func<Task> act = async () => await _redmineClient.GetTimeEntryActivitiesAsync();

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.Message.Contains("404"));
        }

        [Fact]
        public async Task EnsureSuccessAndLogAsync_WhenResponseIsSuccessful_ShouldNotThrow()
        {
            // Arrange
            SetupHttpResponse("/users.json", "{}");

            // Act & Assert
            Func<Task> act = async () => await _redmineClient.GetUsersAsync();
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task EnsureSuccessAndLogAsync_WhenResponseIsUnauthorized_ShouldLogErrorAndThrow()
        {
            // Arrange
            SetupHttpErrorResponse("/users.json", HttpStatusCode.Unauthorized, "Unauthorized access");

            // Act
            Func<Task> act = async () => await _redmineClient.GetUsersAsync();

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.Message.Contains("401"));

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v!.ToString().Contains("Redmine API request failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task EnsureSuccessAndLogAsync_WhenResponseIsForbidden_ShouldLogErrorAndThrow()
        {
            // Arrange
            SetupHttpErrorResponse("/users.json", HttpStatusCode.Forbidden, "Access forbidden");

            // Act
            Func<Task> act = async () => await _redmineClient.GetUsersAsync();

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.Message.Contains("403"));

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v!.ToString().Contains("Redmine API request failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task EnsureSuccessAndLogAsync_WhenResponseIsInternalServerError_ShouldLogErrorAndThrow()
        {
            // Arrange
            SetupHttpErrorResponse("/users.json", HttpStatusCode.InternalServerError, "Internal server error");

            // Act
            Func<Task> act = async () => await _redmineClient.GetUsersAsync();

            // Assert
            await act.Should().ThrowAsync<HttpRequestException>()
                .Where(ex => ex.Message.Contains("500"));

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v!.ToString().Contains("Redmine API request failed")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetUsersAsync_WhenJsonResponseIsMalformed_ShouldHandleGracefully()
        {
            // Arrange
            SetupHttpResponse("/users.json", "{ invalid json }");

            // Act
            Func<Task> act = async () => await _redmineClient.GetUsersAsync();

            // Assert
            await act.Should().ThrowAsync<JsonException>();
        }

        [Fact]
        public async Task GetProjectsAsync_WhenJsonResponseIsMalformed_ShouldHandleGracefully()
        {
            // Arrange
            SetupHttpErrorResponse("/projects.json?status=1", HttpStatusCode.InternalServerError);
            SetupHttpResponse("/projects.json", "{ invalid json }");

            // Act
            Func<Task> act = async () => await _redmineClient.GetProjectsAsync();

            // Assert
            await act.Should().ThrowAsync<JsonException>();
        }

        [Fact]
        public async Task GetTimeEntriesAsync_WhenJsonResponseIsMalformed_ShouldHandleGracefully()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 31);
            SetupHttpResponse($"/time_entries.json?from={fromDate:yyyy-MM-dd}&to={toDate:yyyy-MM-dd}", "{ invalid json }");

            // Act
            Func<Task> act = async () => await _redmineClient.GetTimeEntriesAsync(fromDate, toDate);

            // Assert
            await act.Should().ThrowAsync<JsonException>();
        }

        private void SetupHttpResponse(string url, string content)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req!.RequestUri!.ToString().Contains(url)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }

        private void SetupHttpErrorResponse(string url, HttpStatusCode statusCode, string content = "Error")
        {
            var response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req!.RequestUri!.ToString().Contains(url)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }
    }
}