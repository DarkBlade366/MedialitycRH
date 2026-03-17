using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Rules.Aguinaldo.Handlers;
using Application.Features.Payrolls.Rules.Aguinaldo.Commands;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace HumanResource.UnitTests.Application.Features.Payrolls.Rules
{
    public class ChangeAguinaldoRuleStatusHandlerTests
    {
        private readonly Mock<IAguinaldoRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly ChangeAguinaldoRuleStatusHandler _handler;

        public ChangeAguinaldoRuleStatusHandlerTests()
        {
            _repositoryMock = new Mock<IAguinaldoRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheService>();
            _handler = new ChangeAguinaldoRuleStatusHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingValidRule_ShouldActivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeAguinaldoRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new AguinaldoRule(8.33m, 12);
            rule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<AguinaldoRule>());

            // Act
            await _handler.HandleAsync(command);

            // Assert
            rule.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingAlreadyActiveRule_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeAguinaldoRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new AguinaldoRule(8.33m, 12); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Aguinaldo rule is already active.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingWithAnotherActiveRule_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeAguinaldoRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            var rule = new AguinaldoRule(8.33m, 12);
            rule.Deactivate();

            var anotherActiveRule = new AguinaldoRule(7.5m, 11);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<AguinaldoRule> { anotherActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("There is already an active aguinaldo rule; deactivate it first.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDeactivatingValidRule_ShouldDeactivateSuccessfully()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeAguinaldoRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new AguinaldoRule(8.33m, 12);

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

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
            var command = new ChangeAguinaldoRuleStatusCommand
            {
                Id = ruleId,
                IsActive = false
            };

            var rule = new AguinaldoRule(8.33m, 12);
            rule.Deactivate(); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync(rule);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Aguinaldo rule is already inactive.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenRuleDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var ruleId = Guid.NewGuid();
            var command = new ChangeAguinaldoRuleStatusCommand
            {
                Id = ruleId,
                IsActive = true
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(ruleId))
                .ReturnsAsync((AguinaldoRule?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Aguinaldo rule not found.");

            _repositoryMock.Verify(x => x.GetByIdAsync(ruleId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
