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
    public class SyncRedmineProjectsHandlerTests
    {
        private readonly Mock<IRedmineService> _redmineServiceMock;
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly SyncRedmineProjectsHandler _handler;

        public SyncRedmineProjectsHandlerTests()
        {
            _redmineServiceMock = new Mock<IRedmineService>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new SyncRedmineProjectsHandler(
                _redmineServiceMock.Object,
                _projectRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WhenNoRedmineProjects_ShouldReturnZero()
        {
            // Arrange
            var redmineProjects = new List<RedmineProjectDto>();
            var localProjects = new List<Project>();

            _redmineServiceMock
                .Setup(x => x.GetProjectsAsync())
                .ReturnsAsync(redmineProjects);

            _projectRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(localProjects);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _projectRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>()), Times.Never);
            _projectRepositoryMock.Verify(x => x.Update(It.IsAny<Project>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNewProjectsOnly_ShouldCreateAllProjects()
        {
            // Arrange
            var redmineProjects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 },
                new RedmineProjectDto { Id = 2, Name = "Project B", Status = 5 }
            };

            var localProjects = new List<Project>();

            _redmineServiceMock
                .Setup(x => x.GetProjectsAsync())
                .ReturnsAsync(redmineProjects);

            _projectRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(localProjects);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(2);
            _projectRepositoryMock.Verify(x => x.AddAsync(It.Is<Project>(p =>
                p.RedmineProjectId == 1 && p.Name == "Project A" && p.Status == ProjectStatus.Active)), Times.Once);
            _projectRepositoryMock.Verify(x => x.AddAsync(It.Is<Project>(p =>
                p.RedmineProjectId == 2 && p.Name == "Project B" && p.Status == ProjectStatus.Completed)), Times.Once);
            _projectRepositoryMock.Verify(x => x.Update(It.IsAny<Project>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenExistingProjectsOnly_ShouldUpdateChangedProjects()
        {
            // Arrange
            var redmineProjects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Updated Project A", Status = 5 }, // Name and status changed
                new RedmineProjectDto { Id = 2, Name = "Project B", Status = 1 } // No changes
            };

            var localProjects = new List<Project>
            {
                new Project(1, "Project A", ProjectStatus.Active),
                new Project(2, "Project B", ProjectStatus.Active)
            };

            _redmineServiceMock
                .Setup(x => x.GetProjectsAsync())
                .ReturnsAsync(redmineProjects);

            _projectRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(localProjects);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _projectRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>()), Times.Never);
            _projectRepositoryMock.Verify(x => x.Update(It.Is<Project>(p =>
                p.RedmineProjectId == 1 && p.Name == "Updated Project A" && p.Status == ProjectStatus.Completed)), Times.Exactly(2)); // Once for name, once for status
            _projectRepositoryMock.Verify(x => x.Update(It.Is<Project>(p => p.RedmineProjectId == 2)), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenMixedProjects_ShouldCreateNewAndUpdateExisting()
        {
            // Arrange
            var redmineProjects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 }, // Existing
                new RedmineProjectDto { Id = 3, Name = "New Project C", Status = 9 } // New
            };

            var localProjects = new List<Project>
            {
                new Project(1, "Project A", ProjectStatus.Active)
            };

            _redmineServiceMock
                .Setup(x => x.GetProjectsAsync())
                .ReturnsAsync(redmineProjects);

            _projectRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(localProjects);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _projectRepositoryMock.Verify(x => x.AddAsync(It.Is<Project>(p =>
                p.RedmineProjectId == 3 && p.Name == "New Project C" && p.Status == ProjectStatus.Cancelled)), Times.Once);
            _projectRepositoryMock.Verify(x => x.Update(It.Is<Project>(p => p.RedmineProjectId == 1)), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenProjectsNoLongerInRedmine_ShouldCancelNonCompletedProjects()
        {
            // Arrange
            var redmineProjects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 }
            };

            var localProjects = new List<Project>
            {
                new Project(1, "Project A", ProjectStatus.Active), // Should NOT be cancelled
                new Project(2, "Project B", ProjectStatus.Active), // Should be cancelled
                new Project(3, "Project C", ProjectStatus.Completed) // Should NOT be cancelled
            };

            _redmineServiceMock
                .Setup(x => x.GetProjectsAsync())
                .ReturnsAsync(redmineProjects);

            _projectRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(localProjects);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _projectRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>()), Times.Never);
            _projectRepositoryMock.Verify(x => x.Update(It.Is<Project>(p =>
                p.RedmineProjectId == 2 && p.Status == ProjectStatus.Cancelled)), Times.Once);
            _projectRepositoryMock.Verify(x => x.Update(It.Is<Project>(p => p.RedmineProjectId == 1)), Times.Never);
            _projectRepositoryMock.Verify(x => x.Update(It.Is<Project>(p => p.RedmineProjectId == 3)), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(1, ProjectStatus.Active)]
        [InlineData(5, ProjectStatus.Completed)]
        [InlineData(9, ProjectStatus.Cancelled)]
        [InlineData(99, ProjectStatus.Active)] // Unknown status defaults to Active
        public async Task Handle_WithDifferentStatuses_ShouldMapCorrectly(int redmineStatus, ProjectStatus expectedStatus)
        {
            // Arrange
            var redmineProjects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Test Project", Status = redmineStatus }
            };

            var localProjects = new List<Project>();

            _redmineServiceMock
                .Setup(x => x.GetProjectsAsync())
                .ReturnsAsync(redmineProjects);

            _projectRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(localProjects);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _projectRepositoryMock.Verify(x => x.AddAsync(It.Is<Project>(p =>
                p.RedmineProjectId == 1 && p.Name == "Test Project" && p.Status == expectedStatus)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRedmineServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            _redmineServiceMock
                .Setup(x => x.GetProjectsAsync())
                .ThrowsAsync(new Exception("Redmine API error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(CancellationToken.None));
            exception.Message.Should().Be("Redmine API error");
            
            _projectRepositoryMock.Verify(x => x.GetAllAsync(), Times.Never);
            _projectRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>()), Times.Never);
            _projectRepositoryMock.Verify(x => x.Update(It.IsAny<Project>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var redmineProjects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 }
            };

            _redmineServiceMock
                .Setup(x => x.GetProjectsAsync())
                .ReturnsAsync(redmineProjects);

            _projectRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(CancellationToken.None));
            exception.Message.Should().Be("Database error");
            
            _projectRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>()), Times.Never);
            _projectRepositoryMock.Verify(x => x.Update(It.IsAny<Project>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUnitOfWorkThrowsException_ShouldPropagateException()
        {
            // Arrange
            var redmineProjects = new List<RedmineProjectDto>
            {
                new RedmineProjectDto { Id = 1, Name = "Project A", Status = 1 }
            };

            var localProjects = new List<Project>();

            _redmineServiceMock
                .Setup(x => x.GetProjectsAsync())
                .ReturnsAsync(redmineProjects);

            _projectRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(localProjects);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Save error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(CancellationToken.None));
            exception.Message.Should().Be("Save error");
            
            _projectRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
