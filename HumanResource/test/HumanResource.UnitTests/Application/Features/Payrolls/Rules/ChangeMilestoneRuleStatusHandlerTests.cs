using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Rules.Milestones.Handlers;
using Application.Features.Payrolls.Rules.Milestones.Commands;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace HumanResource.UnitTests.Application.Features.Payrolls.Rules
{
    public class ChangeMilestoneRuleStatusHandlerTests
    {
        private readonly Mock<IMilestoneRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ChangeMilestoneRuleStatusHandler _handler;

        public ChangeMilestoneRuleStatusHandlerTests()
        {
            _repositoryMock = new Mock<IMilestoneRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ChangeMilestoneRuleStatusHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingValidRule_ShouldActivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeMilestoneRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new MilestoneRule(123, "Phase 1", 2000m);
            rule.Deactivate(); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule>());

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
            var command = new ChangeMilestoneRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new MilestoneRule(123, "Phase 1", 2000m); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Milestone rule is already active.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDeactivatingValidRule_ShouldDeactivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeMilestoneRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new MilestoneRule(123, "Phase 1", 2000m); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule>());

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
            var command = new ChangeMilestoneRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new MilestoneRule(123, "Phase 1", 2000m);
            rule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Milestone rule is already inactive.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenRuleDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeMilestoneRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync((MilestoneRule?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Milestone rule not found.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(123, "Phase 1", 1000)]
        [InlineData(456, "Phase 2", 5000)]
        [InlineData(789, "Milestone A", 10000)]
        public async Task HandleAsync_WithDifferentProjects_ShouldChangeStatusSuccessfully(int projectId, string milestoneName, int bonusAmount)
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeMilestoneRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new MilestoneRule(projectId, milestoneName, (decimal)bonusAmount);
            rule.Deactivate(); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();
            rule.RedmineProjectId.Should().Be(projectId);
            rule.MilestoneName.Should().Be(milestoneName);
            rule.BonusAmount.Should().Be((decimal)bonusAmount);
            _repositoryMock.Verify(x => x.Update(rule), Times.Once);
            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(5000)]
        [InlineData(10000)]
        public async Task HandleAsync_WithDifferentBonusAmounts_ShouldChangeStatusSuccessfully(int bonusAmount)
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeMilestoneRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new MilestoneRule(123, "Phase 1", (decimal)bonusAmount);
            rule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();
            rule.BonusAmount.Should().Be((decimal)bonusAmount);
            _repositoryMock.Verify(x => x.Update(rule), Times.Once);
            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingWithAnotherActiveRuleForSameProjectAndName_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeMilestoneRuleStatusCommand { Id = ruleId, IsActive = true };
            var rule = new MilestoneRule(123, "Phase 1", 2000m);
            rule.Deactivate();

            var anotherActiveRule = new MilestoneRule(123, "Phase 1", 1500m);

            _repositoryMock.Setup(x => x.GetByIdAsync(ruleId)).ReturnsAsync(rule);
            _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<MilestoneRule> { anotherActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
            exception.Message.Should().Be("Another active milestone rule already exists for project 123 and milestone 'Phase 1'. deactivate it first.");

            _repositoryMock.Verify(x => x.Update(It.IsAny<MilestoneRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
