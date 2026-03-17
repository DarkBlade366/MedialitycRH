using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Rules.Productivity.Handlers;
using Application.Features.Payrolls.Rules.Productivity.Commands;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Enums;

namespace HumanResource.UnitTests.Application.Features.Payrolls.Rules
{
    public class ChangeProductivityRuleStatusHandlerTests
    {
        private readonly Mock<IProductivityRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly ChangeProductivityRuleStatusHandler _handler;

        public ChangeProductivityRuleStatusHandlerTests()
        {
            _repositoryMock = new Mock<IProductivityRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheService>();
            _handler = new ChangeProductivityRuleStatusHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingValidRule_ShouldActivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProductivityRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new ProductivityRule(50m, 100m, 500m, BonusType.FixedAmount);
            rule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductivityRule>());

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
            var command = new ChangeProductivityRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new ProductivityRule(50m, 100m, 500m, BonusType.FixedAmount);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductivityRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Productivity rule is already active.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDeactivatingValidRule_ShouldDeactivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProductivityRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new ProductivityRule(50m, 100m, 500m, BonusType.FixedAmount);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductivityRule>());

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
            var command = new ChangeProductivityRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new ProductivityRule(50m, 100m, 500m, BonusType.FixedAmount);
            rule.Deactivate(); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductivityRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Productivity rule is already inactive.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenRuleDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProductivityRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync((ProductivityRule?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Productivity rule not found.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(BonusType.FixedAmount, 50, 100, 500)]
        [InlineData(BonusType.Percentage, 30, 80, 15)]
        public async Task HandleAsync_WithDifferentTypes_ShouldChangeStatusSuccessfully(BonusType type, int minimum, int full, int bonus)
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProductivityRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new ProductivityRule(minimum, full, bonus, type);
            rule.Deactivate(); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductivityRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();
            rule.BonusType.Should().Be(type);
            rule.MinimumTarget.Should().Be(minimum);
            rule.FullBonusTarget.Should().Be(full);
            rule.BonusValue.Should().Be(bonus);
            _repositoryMock.Verify(x => x.Update(rule), Times.Once);
            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingWithAnotherActiveRule_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeProductivityRuleStatusCommand { Id = ruleId, IsActive = true };
            var rule = new ProductivityRule(50m, 100m, 500m, BonusType.FixedAmount);
            rule.Deactivate();

            var anotherActiveRule = new ProductivityRule(30m, 80m, 200m, BonusType.Percentage);

            _repositoryMock.Setup(x => x.GetByIdAsync(ruleId)).ReturnsAsync(rule);
            _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ProductivityRule> { anotherActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
            exception.Message.Should().Be("There is already an active productivity rule; deactivate it before activating this one.");

            _repositoryMock.Verify(x => x.Update(It.IsAny<ProductivityRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
