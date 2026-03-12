using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Rules.Deduction.Handlers;
using Application.Features.Payrolls.Rules.Deduction.Commands;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Enums;

namespace Application.Features.Payrolls.Rules
{
    public class ChangeDeductionRuleStatusHandlerTests
    {
        private readonly Mock<IDeductionRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ChangeDeductionRuleStatusHandler _handler;

        public ChangeDeductionRuleStatusHandlerTests()
        {
            _repositoryMock = new Mock<IDeductionRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ChangeDeductionRuleStatusHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingValidRule_ShouldActivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeDeductionRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new DeductionRule(0.15m, "Tax", DeductionType.BasicSalary);
            rule.Deactivate(); // Start as inactive

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule>());

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
            var command = new ChangeDeductionRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new DeductionRule(0.15m, "Tax", DeductionType.BasicSalary); // Already active

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Deduction rule is already active.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDeactivatingValidRule_ShouldDeactivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeDeductionRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new DeductionRule(0.15m, "Tax", DeductionType.BasicSalary); // Active

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule>());

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
            var command = new ChangeDeductionRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new DeductionRule(0.15m, "Tax", DeductionType.BasicSalary);
            rule.Deactivate(); // Already inactive

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Deduction rule is already inactive.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenRuleDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeDeductionRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync((DeductionRule)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Deduction rule not found.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(DeductionType.BasicSalary, 15, "Tax")]
        [InlineData(DeductionType.TotalEarnings, 8, "Social Security")]
        [InlineData(DeductionType.BasicSalary, 10, "Health Insurance")]
        public async Task HandleAsync_WithDifferentTypes_ShouldChangeStatusSuccessfully(DeductionType type, int percentageInt, string description)
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeDeductionRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new DeductionRule(percentageInt / 100m, "Test", type);
            rule.Deactivate(); // Start as inactive

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();
            rule.Type.Should().Be(type);
            rule.Percentage.Should().Be(percentageInt / 100m);

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("Tax")]
        [InlineData("Social Security")]
        [InlineData("Health Insurance")]
        public async Task HandleAsync_WithDifferentDescriptions_ShouldChangeStatusSuccessfully(string description)
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeDeductionRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new DeductionRule(0.15m, description, DeductionType.BasicSalary);
            rule.Deactivate(); // Start as inactive

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();
            rule.Description.Should().Be(description);

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingWithAnotherActiveRuleForSameTypeAndDescription_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeDeductionRuleStatusCommand { Id = ruleId, IsActive = true };
            var rule = new DeductionRule(0.15m, "Tax", DeductionType.BasicSalary);
            rule.Deactivate();

            var anotherActiveRule = new DeductionRule(0.10m, "Tax", DeductionType.BasicSalary); // mismo tipo y descripción, activa

            _repositoryMock.Setup(x => x.GetByIdAsync(ruleId)).ReturnsAsync(rule);
            _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<DeductionRule> { anotherActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
            exception.Message.Should().Be("Another active deduction rule with type 'BasicSalary' and description 'Tax' already exists; deactivate it first.");

            _repositoryMock.Verify(x => x.Update(It.IsAny<DeductionRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
