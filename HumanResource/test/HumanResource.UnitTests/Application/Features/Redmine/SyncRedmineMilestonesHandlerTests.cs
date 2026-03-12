using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Redmine.Handlers;
using Application.Features.Redmine.Interfaces;
using Application.Features.Redmine.DTOs;
using Domain.Features.Projects.Interfaces;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Enums;
using Application.Common.Interfaces;

namespace Application.Features.Redmine
{
    public class SyncRedmineMilestonesHandlerTests
    {
        private readonly Mock<IRedmineService> _redmineServiceMock;
        private readonly Mock<IProjectMilestoneRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly SyncRedmineMilestonesHandler _handler;

        public SyncRedmineMilestonesHandlerTests()
        {
            _redmineServiceMock = new Mock<IRedmineService>();
            _repositoryMock = new Mock<IProjectMilestoneRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new SyncRedmineMilestonesHandler(
                _redmineServiceMock.Object,
                _repositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WhenNoProjects_ShouldReturnZero()
        {
            // Arrange
            var projects = new List<RedmineProjectDto>();

            _redmineServiceMock
                .Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _redmineServiceMock.Verify(x => x.GetProjectMilestonesAsync(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(x => x.GetByProjectIdAsync(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<ProjectMilestone>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProjectsWithNoMilestones_ShouldReturnZero()
        {
            // Arrange
            var projects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 }
            };

            var milestones = new List<RedmineMilestoneDto>();
            var existingMilestones = new List<ProjectMilestone>();

            _redmineServiceMock
                .Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            _redmineServiceMock
                .Setup(x => x.GetProjectMilestonesAsync(1))
                .ReturnsAsync(milestones);

            _repositoryMock
                .Setup(x => x.GetByProjectIdAsync(1))
                .ReturnsAsync(existingMilestones);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _repositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<ProjectMilestone>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNewMilestonesOnly_ShouldCreateAllMilestones()
        {
            // Arrange
            var projects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 }
            };

            var milestones = new List<RedmineMilestoneDto>
            {
                new RedmineMilestoneDto { ProjectId = 1, Name = "Milestone 1", Status = "open" },
                new RedmineMilestoneDto { ProjectId = 1, Name = "Milestone 2", Status = "closed", CompletedAt = DateTime.Now }
            };

            var existingMilestones = new List<ProjectMilestone>();

            _redmineServiceMock
                .Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            _redmineServiceMock
                .Setup(x => x.GetProjectMilestonesAsync(1))
                .ReturnsAsync(milestones);

            _repositoryMock
                .Setup(x => x.GetByProjectIdAsync(1))
                .ReturnsAsync(existingMilestones);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(2);
            _repositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<ProjectMilestone>>(ms =>
                ms.Count == 2 &&
                ms.Any(m => m.Name == "Milestone 1" && m.RedmineProjectId == 1 && !m.IsCompleted() && !m.IsCancelled()) &&
                ms.Any(m => m.Name == "Milestone 2" && m.RedmineProjectId == 1 && m.IsCompleted()))), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenExistingMilestonesOnly_ShouldUpdateStatusChanges()
        {
            // Arrange
            var projects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 }
            };

            var milestones = new List<RedmineMilestoneDto>
            {
                new RedmineMilestoneDto { ProjectId = 1, Name = "Milestone 1", Status = "closed", CompletedAt = DateTime.Now }, // Should be marked as completed
                new RedmineMilestoneDto { ProjectId = 1, Name = "Milestone 2", Status = "locked" }, // Should be marked as cancelled
                new RedmineMilestoneDto { ProjectId = 1, Name = "Milestone 3", Status = "open" } // Should be reopened if was completed/cancelled
            };

            var existingMilestones = new List<ProjectMilestone>
            {
                new ProjectMilestone(1, "Milestone 1"), // Pending
                new ProjectMilestone(1, "Milestone 2"), // Pending
                new ProjectMilestone(1, "Milestone 3") // Will be marked as completed
            };
            // Mark the third milestone as completed
            existingMilestones[2].MarkAsCompleted(DateTime.Now.AddDays(-1));

            _redmineServiceMock
                .Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            _redmineServiceMock
                .Setup(x => x.GetProjectMilestonesAsync(1))
                .ReturnsAsync(milestones);

            _repositoryMock
                .Setup(x => x.GetByProjectIdAsync(1))
                .ReturnsAsync(existingMilestones);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _repositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<ProjectMilestone>>()), Times.Never);
            
            // Verify status updates
            existingMilestones.First(m => m.Name == "Milestone 1").IsCompleted().Should().BeTrue();
            existingMilestones.First(m => m.Name == "Milestone 2").IsCancelled().Should().BeTrue();
            existingMilestones.First(m => m.Name == "Milestone 3").IsPending().Should().BeTrue(); // Reopened
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenMixedMilestones_ShouldCreateNewAndUpdateExisting()
        {
            // Arrange
            var projects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 }
            };

            var milestones = new List<RedmineMilestoneDto>
            {
                new RedmineMilestoneDto { ProjectId = 1, Name = "Existing Milestone", Status = "closed", CompletedAt = DateTime.Now }, // Existing
                new RedmineMilestoneDto { ProjectId = 1, Name = "New Milestone", Status = "open" } // New
            };

            var existingMilestones = new List<ProjectMilestone>
            {
                new ProjectMilestone(1, "Existing Milestone") // Pending
            };

            _redmineServiceMock
                .Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            _redmineServiceMock
                .Setup(x => x.GetProjectMilestonesAsync(1))
                .ReturnsAsync(milestones);

            _repositoryMock
                .Setup(x => x.GetByProjectIdAsync(1))
                .ReturnsAsync(existingMilestones);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _repositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<ProjectMilestone>>(ms =>
                ms.Count == 1 &&
                ms.Any(m => m.Name == "New Milestone" && m.RedmineProjectId == 1 && m.IsPending()))), Times.Once);
            
            // Verify existing milestone was updated
            existingMilestones.First(m => m.Name == "Existing Milestone").IsCompleted().Should().BeTrue();
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("open", MilestoneStatus.Pending)]
        [InlineData("closed", MilestoneStatus.Completed)]
        [InlineData("locked", MilestoneStatus.Cancelled)]
        [InlineData("OPEN", MilestoneStatus.Pending)] // Case insensitive
        [InlineData("CLOSED", MilestoneStatus.Completed)]
        [InlineData("LOCKED", MilestoneStatus.Cancelled)]
        [InlineData("", MilestoneStatus.Pending)] // Empty string
        [InlineData("unknown", MilestoneStatus.Pending)] // Unknown status
        public async Task Handle_WithDifferentStatuses_ShouldMapCorrectly(string redmineStatus, MilestoneStatus expectedStatus)
        {
            // Arrange
            var projects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 }
            };

            var milestones = new List<RedmineMilestoneDto>
            {
                new RedmineMilestoneDto { ProjectId = 1, Name = "Test Milestone", Status = redmineStatus, CompletedAt = DateTime.Now }
            };

            var existingMilestones = new List<ProjectMilestone>();

            _redmineServiceMock
                .Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            _redmineServiceMock
                .Setup(x => x.GetProjectMilestonesAsync(1))
                .ReturnsAsync(milestones);

            _repositoryMock
                .Setup(x => x.GetByProjectIdAsync(1))
                .ReturnsAsync(existingMilestones);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _repositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<ProjectMilestone>>(ms =>
                ms.Count == 1 &&
                ms.Any(m => m.Name == "Test Milestone" && m.RedmineProjectId == 1 && m.Status == expectedStatus))), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenMultipleProjects_ShouldProcessAllProjects()
        {
            // Arrange
            var projects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 },
                new RedmineProjectDto { Id = 2, Name = "Project B", Status = 1 }
            };

            var project1Milestones = new List<RedmineMilestoneDto>
            {
                new RedmineMilestoneDto { ProjectId = 1, Name = "Milestone A1", Status = "open" }
            };

            var project2Milestones = new List<RedmineMilestoneDto>
            {
                new RedmineMilestoneDto { ProjectId = 2, Name = "Milestone B1", Status = "open" },
                new RedmineMilestoneDto { ProjectId = 2, Name = "Milestone B2", Status = "open" }
            };

            _redmineServiceMock
                .Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            _redmineServiceMock
                .Setup(x => x.GetProjectMilestonesAsync(1))
                .ReturnsAsync(project1Milestones);

            _redmineServiceMock
                .Setup(x => x.GetProjectMilestonesAsync(2))
                .ReturnsAsync(project2Milestones);

            _repositoryMock
                .Setup(x => x.GetByProjectIdAsync(1))
                .ReturnsAsync(new List<ProjectMilestone>());

            _repositoryMock
                .Setup(x => x.GetByProjectIdAsync(2))
                .ReturnsAsync(new List<ProjectMilestone>());

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(3); // 1 from project 1 + 2 from project 2
            _repositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<ProjectMilestone>>(ms => ms.Count == 1 &&
                ms.Any(m => m.Name == "Milestone A1" && m.RedmineProjectId == 1))), Times.Once);
            _repositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<ProjectMilestone>>(ms => ms.Count == 2 &&
                ms.Any(m => m.Name == "Milestone B1" && m.RedmineProjectId == 2) &&
                ms.Any(m => m.Name == "Milestone B2" && m.RedmineProjectId == 2))), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRedmineServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            _redmineServiceMock
                .Setup(x => x.GetAllProjectsAsync())
                .ThrowsAsync(new Exception("Redmine API error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(CancellationToken.None));
            exception.Message.Should().Be("Redmine API error");
            
            _redmineServiceMock.Verify(x => x.GetProjectMilestonesAsync(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(x => x.GetByProjectIdAsync(It.IsAny<int>()), Times.Never);
            _repositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<ProjectMilestone>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var projects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 }
            };

            var milestones = new List<RedmineMilestoneDto>
            {
                new RedmineMilestoneDto { ProjectId = 1, Name = "Test Milestone", Status = "open" }
            };

            _redmineServiceMock
                .Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            _redmineServiceMock
                .Setup(x => x.GetProjectMilestonesAsync(1))
                .ReturnsAsync(milestones);

            _repositoryMock
                .Setup(x => x.GetByProjectIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(CancellationToken.None));
            exception.Message.Should().Be("Database error");
            
            _repositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<ProjectMilestone>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUnitOfWorkThrowsException_ShouldPropagateException()
        {
            // Arrange
            var projects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 }
            };

            _redmineServiceMock
                .Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            _redmineServiceMock
                .Setup(x => x.GetProjectMilestonesAsync(1))
                .ReturnsAsync(new List<RedmineMilestoneDto>());

            _repositoryMock
                .Setup(x => x.GetByProjectIdAsync(1))
                .ReturnsAsync(new List<ProjectMilestone>());

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Save error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(CancellationToken.None));
            exception.Message.Should().Be("Save error");
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
