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
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace HumanResource.UnitTests.Application.Features.Payrolls.Rules
{
    public class ChangeProjectRuleStatusHandlerTests
    {
        private readonly Mock<IProjectRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly ChangeProjectRuleStatusHandler _handler;

        public ChangeProjectRuleStatusHandlerTests()
        {
            _repositoryMock = new Mock<IProjectRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheService>();
            _handler = new ChangeProjectRuleStatusHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingValidRule_ShouldActivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProjectRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new ProjectRule(123, 3000m);
            rule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProjectRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();
            _repositoryMock.Verify(x => x.Update(rule), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingAlreadyActiveRule_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProjectRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new ProjectRule(123, 3000m);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProjectRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Project rule is already active.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDeactivatingValidRule_ShouldDeactivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProjectRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new ProjectRule(123, 3000m);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProjectRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeFalse();
            _repositoryMock.Verify(x => x.Update(rule), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenDeactivatingAlreadyInactiveRule_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProjectRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new ProjectRule(123, 3000m);
            rule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProjectRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Project rule is already inactive.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenRuleDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProjectRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync((ProjectRule?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Project rule not found.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(123, 1000)]
        [InlineData(456, 5000)]
        [InlineData(789, 10000)]
        public async Task HandleAsync_WithDifferentProjects_ShouldChangeStatusSuccessfully(int projectId, int bonusAmount)
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProjectRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new ProjectRule(projectId, (decimal)bonusAmount);
            rule.Deactivate(); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProjectRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();
            rule.RedmineProjectId.Should().Be(projectId);
            rule.BonusAmount.Should().Be((decimal)bonusAmount);
            _repositoryMock.Verify(x => x.Update(rule), Times.Once);
            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingWithAnotherActiveRuleForSameProject_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProjectRuleStatusCommand { Id = ruleId, IsActive = true };
            var rule = new ProjectRule(123, 3000m);
            rule.Deactivate();

            var anotherActiveRule = new ProjectRule(123, 5000m); 

            _repositoryMock.Setup(x => x.GetByIdAsync(ruleId)).ReturnsAsync(rule);
            _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ProjectRule> { anotherActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
            exception.Message.Should().Be("Another active project rule already exists for project 123. deactivate it first.");

            _repositoryMock.Verify(x => x.Update(It.IsAny<ProjectRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
