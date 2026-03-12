using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Rules.Aguinaldo.Handlers;
using Application.Features.Payrolls.Rules.Aguinaldo.Commands;
using Application.Features.Payrolls.Rules.Aguinaldo.DTOs;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules
{
    public class CreateAguinaldoRuleHandlerTests
    {
        private readonly Mock<IAguinaldoRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateAguinaldoRuleHandler _handler;

        public CreateAguinaldoRuleHandlerTests()
        {
            _repositoryMock = new Mock<IAguinaldoRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateAguinaldoRuleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenValidData_ShouldCreateRuleSuccessfully()
        {
            // Arrange
            var command = new CreateAguinaldoRuleCommand
            {
                MonthlyAccrualPercentage = 8.33m,
                PayMonth = 12
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<AguinaldoRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.MonthlyAccrualPercentage.Should().Be(8.33m);
            result.PayMonth.Should().Be(12);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<AguinaldoRule>(r =>
                r.MonthlyAccrualPercentage == 8.33m &&
                r.PayMonth == 12 &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateAguinaldoRuleCommand
            {
                MonthlyAccrualPercentage = 8.33m,
                PayMonth = 12
            };

            var existingActiveRule = new AguinaldoRule(8.33m, 12);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<AguinaldoRule> { existingActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("There is already an active aguinaldo rule.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<AguinaldoRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenIdenticalInactiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateAguinaldoRuleCommand
            {
                MonthlyAccrualPercentage = 8.33m,
                PayMonth = 12
            };

            var existingInactiveRule = new AguinaldoRule(8.33m, 12);
            existingInactiveRule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<AguinaldoRule> { existingInactiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Contain("An aguinaldo rule with 8,33% monthly accrual for month 12 already exists but is disabled. Enable it instead of creating a new one.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<AguinaldoRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDifferentRulesExist_ShouldThrowException()
        {
            // Arrange
            var command = new CreateAguinaldoRuleCommand
            {
                MonthlyAccrualPercentage = 8.33m,
                PayMonth = 12
            };

            var existingDifferentRule = new AguinaldoRule(7.5m, 11);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<AguinaldoRule> { existingDifferentRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("There is already an active aguinaldo rule.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<AguinaldoRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
