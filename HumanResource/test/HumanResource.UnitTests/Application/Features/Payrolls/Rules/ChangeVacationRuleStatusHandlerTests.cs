using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Rules.Vacation.Handlers;
using Application.Features.Payrolls.Rules.Vacation.Commands;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;

namespace HumanResource.UnitTests.Application.Features.Payrolls.Rules
{
    public class ChangeVacationRuleStatusHandlerTests
    {
        private readonly Mock<IVacationRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly ChangeVacationRuleStatusHandler _handler;

        public ChangeVacationRuleStatusHandlerTests()
        {
            _repositoryMock = new Mock<IVacationRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheService>();
            _handler = new ChangeVacationRuleStatusHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingValidRule_ShouldActivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeVacationRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new VacationRule(1.25m);
            rule.Deactivate(); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingAlreadyActiveRule_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeVacationRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new VacationRule(1.25m); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Vacation rule is already active.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDeactivatingValidRule_ShouldDeactivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeVacationRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new VacationRule(1.25m);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeFalse();

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenDeactivatingAlreadyInactiveRule_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeVacationRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new VacationRule(1.25m);
            rule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Vacation rule is already inactive.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenRuleDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeVacationRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync((VacationRule?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Vacation rule not found.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(125)]
        [InlineData(167)]
        [InlineData(208)]
        public async Task HandleAsync_WithDifferentRoles_ShouldChangeStatusSuccessfully(int accrualRateInt)
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeVacationRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new VacationRule(accrualRateInt / 100m);
            rule.Deactivate(); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();
            rule.AccrualRatePerMonth.Should().Be(accrualRateInt / 100m);
            _repositoryMock.Verify(x => x.Update(rule), Times.Once);
            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(83)]
        [InlineData(125)]
        [InlineData(167)]
        [InlineData(208)]
        public async Task HandleAsync_WithDifferentYearsOfService_ShouldChangeStatusSuccessfully(int accrualRateInt)
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeVacationRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new VacationRule(accrualRateInt / 100m);
            rule.Deactivate(); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();
            rule.AccrualRatePerMonth.Should().Be(accrualRateInt / 100m);
            _repositoryMock.Verify(x => x.Update(rule), Times.Once);
            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingWithAnotherActiveRule_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeVacationRuleStatusCommand { Id = ruleId, IsActive = true };
            var rule = new VacationRule(1.25m);
            rule.Deactivate();

            var anotherActiveRule = new VacationRule(1.5m);
            
            _repositoryMock.Setup(x => x.GetByIdAsync(ruleId)).ReturnsAsync(rule);
            _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<VacationRule> { anotherActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
            exception.Message.Should().Be("There is already an active vacation rule; deactivate it before activating this one.");

            _repositoryMock.Verify(x => x.Update(It.IsAny<VacationRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
