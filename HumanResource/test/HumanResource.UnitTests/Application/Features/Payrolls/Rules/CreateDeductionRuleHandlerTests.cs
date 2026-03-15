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
using Application.Features.Payrolls.Rules.Deduction.DTOs;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Enums;

namespace HumanResource.UnitTests.Application.Features.Payrolls.Rules
{
    public class CreateDeductionRuleHandlerTests
    {
        private readonly Mock<IDeductionRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateDeductionRuleHandler _handler;

        public CreateDeductionRuleHandlerTests()
        {
            _repositoryMock = new Mock<IDeductionRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateDeductionRuleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenValidData_ShouldCreateRuleSuccessfully()
        {
            // Arrange
            var command = new CreateDeductionRuleCommand
            {
                Description = "Tax",
                Percentage = 0.15m,
                Type = "BasicSalary"
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Description.Should().Be("Tax");
            result.Percentage.Should().Be(0.15m);
            result.Type.Should().Be("BasicSalary");
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<DeductionRule>(r =>
                r.Description == "Tax" &&
                r.Percentage == 0.15m &&
                r.Type == DeductionType.BasicSalary &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActiveRuleWithSameTypeAndDescriptionExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateDeductionRuleCommand
            {
                Description = "Tax",
                Percentage = 0.15m,
                Type = "BasicSalary"
            };

            var existingActiveRule = new DeductionRule(0.10m, "Tax", DeductionType.BasicSalary);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule> { existingActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("An active deduction rule with type 'BasicSalary' and description 'Tax' already exists.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<DeductionRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenIdenticalInactiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateDeductionRuleCommand
            {
                Description = "Tax",
                Percentage = 0.15m,
                Type = "BasicSalary"
            };

            var existingInactiveRule = new DeductionRule(0.15m, "Tax", DeductionType.BasicSalary);
            existingInactiveRule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule> { existingInactiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Contain("A deduction rule of type 'BasicSalary', description 'Tax' and percentage");
            exception.Message.Should().Contain("already exists but is disabled. Enable it instead of creating a new one.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<DeductionRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDifferentRulesExist_ShouldCreateNewRuleSuccessfully()
        {
            // Arrange
            var command = new CreateDeductionRuleCommand
            {
                Description = "Social Security",
                Percentage = 0.08m,
                Type = "BasicSalary"
            };

            var existingDifferentRule = new DeductionRule(0.15m, "Tax", DeductionType.BasicSalary);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule> { existingDifferentRule });

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Description.Should().Be("Social Security");
            result.Percentage.Should().Be(0.08m);
            result.Type.Should().Be("BasicSalary");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<DeductionRule>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenSameDescriptionDifferentPercentage_ShouldThrowException()
        {
            // Arrange
            var command = new CreateDeductionRuleCommand
            {
                Description = "Tax",
                Percentage = 0.20m,
                Type = "BasicSalary"
            };

            var existingDifferentPercentageRule = new DeductionRule(0.15m, "Tax", DeductionType.BasicSalary);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule> { existingDifferentPercentageRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("An active deduction rule with type 'BasicSalary' and description 'Tax' already exists.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<DeductionRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("BasicSalary", 10)]
        [InlineData("TotalEarnings", 5)]
        public async Task HandleAsync_WithDifferentTypes_ShouldCreateRuleSuccessfully(string type, int percentage)
        {
            // Arrange
            var command = new CreateDeductionRuleCommand
            {
                Description = "Test Deduction",
                Percentage = percentage / 100m,
                Type = type
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Description.Should().Be("Test Deduction");
            result.Percentage.Should().Be(percentage / 100m);
            result.Type.Should().Be(type);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<DeductionRule>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task HandleAsync_WithValidPercentages_ShouldCreateRuleSuccessfully(int percentageInt)
        {
            // Arrange
            var command = new CreateDeductionRuleCommand
            {
                Description = "Test Deduction",
                Percentage = percentageInt / 100m,
                Type = "BasicSalary"
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Percentage.Should().Be(percentageInt / 100m);

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<DeductionRule>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenTypeIsInvalid_ShouldThrowException()
        {
            // Arrange
            var command = new CreateDeductionRuleCommand
            {
                Description = "Test",
                Percentage = 0.1m,
                Type = "InvalidType"
            };

            // Act & Assert
            Func<Task> act = async () => await _handler.HandleAsync(command);
            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
