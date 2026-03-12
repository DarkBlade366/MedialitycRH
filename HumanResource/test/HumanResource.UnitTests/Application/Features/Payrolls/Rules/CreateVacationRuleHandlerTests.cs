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
using Application.Features.Payrolls.Rules.Vacation.DTOs;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Employees.Enums;

namespace Application.Features.Payrolls.Rules
{
    public class CreateVacationRuleHandlerTests
    {
        private readonly Mock<IVacationRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateVacationRuleHandler _handler;

        public CreateVacationRuleHandlerTests()
        {
            _repositoryMock = new Mock<IVacationRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateVacationRuleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenValidData_ShouldCreateRuleSuccessfully()
        {
            // Arrange
            var command = new CreateVacationRuleCommand
            {
                AccrualRatePerMonth = 1.25m
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.AccrualRatePerMonth.Should().Be(1.25m);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<VacationRule>(r =>
                r.AccrualRatePerMonth == 1.25m &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActiveRuleExistsForRoleAndYears_ShouldThrowException()
        {
            // Arrange
            var command = new CreateVacationRuleCommand
            {
                AccrualRatePerMonth = 1.25m
            };

            var existingActiveRule = new VacationRule(1.25m);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule> { existingActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Contain("There is already an active vacation rule with accrual rate");
            exception.Message.Should().Contain("1,25");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<VacationRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenIdenticalInactiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateVacationRuleCommand
            {
                AccrualRatePerMonth = 1.25m
            };

            var existingInactiveRule = new VacationRule(1.25m);
            existingInactiveRule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule> { existingInactiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Contain("A vacation rule with accrual rate");
            exception.Message.Should().Contain("already exists but is disabled. Enable it instead of creating a new one.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<VacationRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDifferentRulesExist_ShouldThrowException()
        {
            // Arrange
            var command = new CreateVacationRuleCommand
            {
                AccrualRatePerMonth = 1.25m
            };

            var existingDifferentRule = new VacationRule(1.67m);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule> { existingDifferentRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Contain("There is already an active vacation rule with accrual rate");
            exception.Message.Should().Contain("1,67");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<VacationRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(0.83)]
        [InlineData(1.25)]
        [InlineData(1.67)]
        [InlineData(2.08)]
        public async Task HandleAsync_WithDifferentAccrualRates_ShouldCreateRuleSuccessfully(decimal accrualRate)
        {
            // Arrange
            var command = new CreateVacationRuleCommand { AccrualRatePerMonth = accrualRate };
            _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<VacationRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.AccrualRatePerMonth.Should().Be(accrualRate);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<VacationRule>(r =>
                r.AccrualRatePerMonth == accrualRate &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
