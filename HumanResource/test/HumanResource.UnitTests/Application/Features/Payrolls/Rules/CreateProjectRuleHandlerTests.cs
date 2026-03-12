using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Rules.Project.Handlers;
using Application.Features.Payrolls.Rules.Project.Commands;
using Application.Features.Payrolls.Rules.Project.DTOs;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Payrolls.Rules
{
    public class CreateProjectRuleHandlerTests
    {
        private readonly Mock<IProjectRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly CreateProjectRuleHandler _handler;

        public CreateProjectRuleHandlerTests()
        {
            _repositoryMock = new Mock<IProjectRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _handler = new CreateProjectRuleHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _projectRepositoryMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenValidData_ShouldCreateRuleSuccessfully()
        {
            // Arrange
            var command = new CreateProjectRuleCommand
            {
                RedmineProjectId = 123,
                BonusAmount = 3000m
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(true);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProjectRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.RedmineProjectId.Should().Be(123);
            result.BonusAmount.Should().Be(3000m);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<ProjectRule>(r =>
                r.RedmineProjectId == 123 &&
                r.BonusAmount == 3000m &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenProjectDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var command = new CreateProjectRuleCommand
            {
                RedmineProjectId = 123,
                BonusAmount = 3000m
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Project with Id 123 does not exist.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<ProjectRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenActiveRuleExistsForProject_ShouldThrowException()
        {
            // Arrange
            var command = new CreateProjectRuleCommand
            {
                RedmineProjectId = 123,
                BonusAmount = 3000m
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(true);

            var existingActiveRule = new ProjectRule(123, 2500m);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProjectRule> { existingActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Contain("There is already an active project rule for project 123 with bonus");
            exception.Message.Should().Contain("disable it before creating a different one.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<ProjectRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenIdenticalInactiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateProjectRuleCommand
            {
                RedmineProjectId = 123,
                BonusAmount = 3000m
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(true);

            var existingInactiveRule = new ProjectRule(123, 3000m);
            existingInactiveRule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProjectRule> { existingInactiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Contain("A project rule for project 123 with bonus 3000 already exists but is disabled.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<ProjectRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(123, 1000)]
        [InlineData(456, 5000)]
        [InlineData(789, 10000)]
        public async Task HandleAsync_WithDifferentProjects_ShouldCreateRuleSuccessfully(int projectId, int bonusAmount)
        {
            // Arrange
            var command = new CreateProjectRuleCommand
            {
                RedmineProjectId = projectId,
                BonusAmount = (decimal)bonusAmount
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(projectId))
                .ReturnsAsync(true);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProjectRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.RedmineProjectId.Should().Be(projectId);
            result.BonusAmount.Should().Be((decimal)bonusAmount);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<ProjectRule>(r =>
                r.RedmineProjectId == projectId &&
                r.BonusAmount == (decimal)bonusAmount &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
