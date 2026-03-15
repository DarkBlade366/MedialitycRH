using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Rules.Overtime.Handlers;
using Application.Features.Payrolls.Rules.Overtime.Commands;
using Application.Features.Payrolls.Rules.Overtime.DTOs;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace HumanResource.UnitTests.Application.Features.Payrolls.Rules
{
    public class CreateOvertimeRuleHandlerTests
    {
        private readonly Mock<IOvertimeRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateOvertimeRuleHandler _handler;

        public CreateOvertimeRuleHandlerTests()
        {
            _repositoryMock = new Mock<IOvertimeRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateOvertimeRuleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenValidData_ShouldCreateRuleSuccessfully()
        {
            // Arrange
            var command = new CreateOvertimeRuleCommand
            {
                StandardHoursPerPeriod = 160,
                OvertimeMultiplier = 1.5m,
                HourlyRate = 25m
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<OvertimeRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.StandardHoursPerPeriod.Should().Be(160);
            result.OvertimeMultiplier.Should().Be(1.5m);
            result.HourlyRate.Should().Be(25m);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<OvertimeRule>(r =>
                r.StandardHoursPerPeriod == 160 &&
                r.OvertimeMultiplier == 1.5m &&
                r.HourlyRate == 25m &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateOvertimeRuleCommand
            {
                StandardHoursPerPeriod = 160,
                OvertimeMultiplier = 1.5m,
                HourlyRate = 25m
            };

            var existingActiveRule = new OvertimeRule(160, 1.5m, 25m);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<OvertimeRule> { existingActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("There is already an active overtime rule; only one can be active at a time.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<OvertimeRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenIdenticalInactiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateOvertimeRuleCommand
            {
                StandardHoursPerPeriod = 160,
                OvertimeMultiplier = 1.5m,
                HourlyRate = 25m
            };

            var existingInactiveRule = new OvertimeRule(160, 1.5m, 25m);
            existingInactiveRule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<OvertimeRule> { existingInactiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Contain("An overtime rule with 160 standard hours, multiplier 1,5 and rate");
            exception.Message.Should().Contain("already exists but is disabled. Enable it instead of creating a new one.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<OvertimeRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDifferentInactiveRulesExist_ShouldCreateNewRuleSuccessfully()
        {
            // Arrange
            var command = new CreateOvertimeRuleCommand
            {
                StandardHoursPerPeriod = 160,
                OvertimeMultiplier = 1.5m,
                HourlyRate = 25m
            };

            var existingDifferentRule = new OvertimeRule(140, 1.25m, 20m);
            existingDifferentRule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<OvertimeRule> { existingDifferentRule });

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.StandardHoursPerPeriod.Should().Be(160);
            result.OvertimeMultiplier.Should().Be(1.5m);
            result.HourlyRate.Should().Be(25m);

            _repositoryMock.Verify(x => x.AddAsync(It.Is<OvertimeRule>(r =>
                r.StandardHoursPerPeriod == 160 &&
                r.OvertimeMultiplier == 1.5m &&
                r.HourlyRate == 25m &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(160, 15, 25)]
        [InlineData(140, 125, 20)]
        [InlineData(168, 200, 30)]
        public async Task HandleAsync_WithDifferentParameters_ShouldCreateRuleSuccessfully(int standardHours, int multiplierInt, int hourlyRate)
        {
            // Arrange
            var command = new CreateOvertimeRuleCommand
            {
                StandardHoursPerPeriod = standardHours,
                OvertimeMultiplier = multiplierInt / 10m,
                HourlyRate = hourlyRate
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<OvertimeRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.StandardHoursPerPeriod.Should().Be(standardHours);
            result.OvertimeMultiplier.Should().Be(multiplierInt / 10m);
            result.HourlyRate.Should().Be((decimal)hourlyRate);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<OvertimeRule>(r =>
                r.StandardHoursPerPeriod == standardHours &&
                r.OvertimeMultiplier == multiplierInt / 10m &&
                r.HourlyRate == (decimal)hourlyRate &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
