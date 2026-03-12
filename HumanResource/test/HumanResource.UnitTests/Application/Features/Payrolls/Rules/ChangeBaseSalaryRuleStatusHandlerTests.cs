using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Rules.BaseSalary.Handlers;
using Application.Features.Payrolls.Rules.BaseSalary.Commands;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Employees.Enums;

namespace Application.Features.Payrolls.Rules
{
    public class ChangeBaseSalaryRuleStatusHandlerTests
    {
        private readonly Mock<IBaseSalaryRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ChangeBaseSalaryRuleStatusHandler _handler;

        public ChangeBaseSalaryRuleStatusHandlerTests()
        {
            _repositoryMock = new Mock<IBaseSalaryRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ChangeBaseSalaryRuleStatusHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingValidRule_ShouldActivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeBaseSalaryRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            rule.Deactivate(); // Start as inactive

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<BaseSalaryRule>());

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
            var command = new ChangeBaseSalaryRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new BaseSalaryRule(EmployeeRole.Employee, 5000m); // Already active

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<BaseSalaryRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Base salary rule is already active.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDeactivatingValidRule_ShouldDeactivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeBaseSalaryRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new BaseSalaryRule(EmployeeRole.Employee, 5000m); // Active

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<BaseSalaryRule>());

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
            var command = new ChangeBaseSalaryRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            rule.Deactivate(); // Already inactive

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<BaseSalaryRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Base salary rule is already inactive.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenRuleDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeBaseSalaryRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync((BaseSalaryRule)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Base salary rule not found.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(EmployeeRole.Employee)]
        [InlineData(EmployeeRole.ProjectManager)]
        [InlineData(EmployeeRole.Administrator)]
        public async Task HandleAsync_WithDifferentRoles_ShouldChangeStatusSuccessfully(EmployeeRole role)
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeBaseSalaryRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new BaseSalaryRule(role, 5000m);
            rule.Deactivate(); // Start as inactive

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<BaseSalaryRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();
            rule.Role.Should().Be(role);
            _repositoryMock.Verify(x => x.Update(rule), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingWithAnotherActiveRuleForSameRole_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeBaseSalaryRuleStatusCommand { Id = ruleId, IsActive = true };
            var rule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            rule.Deactivate();

            var anotherActiveRule = new BaseSalaryRule(EmployeeRole.Employee, 6000m); // Mismo rol, activa por defecto

            _repositoryMock.Setup(x => x.GetByIdAsync(ruleId)).ReturnsAsync(rule);
            _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<BaseSalaryRule> { anotherActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(command));
            exception.Message.Should().Be("Another active base salary rule for role 'Employee' already exists; deactivate it first.");

            _repositoryMock.Verify(x => x.Update(It.IsAny<BaseSalaryRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
