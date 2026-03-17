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
using Application.Features.Payrolls.Rules.Productivity.DTOs;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Enums;

namespace HumanResource.UnitTests.Application.Features.Payrolls.Rules
{
    public class CreateProductivityRuleHandlerTests
    {
        private readonly Mock<IProductivityRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly CreateProductivityRuleHandler _handler;

        public CreateProductivityRuleHandlerTests()
        {
            _repositoryMock = new Mock<IProductivityRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheService>();
            _handler = new CreateProductivityRuleHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenValidData_ShouldCreateRuleSuccessfully()
        {
            // Arrange
            var command = new CreateProductivityRuleCommand
            {
                BonusValue = 500m,
                FullBonusTarget = 100m,
                MinimumTarget = 50m,
                BonusType = "FixedAmount"
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductivityRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.BonusType.Should().Be("FixedAmount");
            result.MinimumTarget.Should().Be(50m);
            result.FullBonusTarget.Should().Be(100m);
            result.BonusValue.Should().Be(500m);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<ProductivityRule>(r =>
                r.MinimumTarget == 50m &&
                r.FullBonusTarget == 100m &&
                r.BonusValue == 500m &&
                r.BonusType == BonusType.FixedAmount &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateProductivityRuleCommand
            {
                BonusValue = 500m,
                FullBonusTarget = 100m,
                MinimumTarget = 50m,
                BonusType = "FixedAmount"
            };

            var existingActiveRule = new ProductivityRule(50m, 100m, 500m, BonusType.FixedAmount);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductivityRule> { existingActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("There is already an active productivity rule; deactivate it first.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<ProductivityRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenIdenticalInactiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateProductivityRuleCommand
            {
                BonusValue = 500m,
                FullBonusTarget = 100m,
                MinimumTarget = 50m,
                BonusType = "FixedAmount"
            };

            var existingInactiveRule = new ProductivityRule(50m, 100m, 500m, BonusType.FixedAmount);
            existingInactiveRule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductivityRule> { existingInactiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Contain("A productivity rule (min 50, full 100, bonus 500 FixedAmount, cap )");
            exception.Message.Should().Contain("already exists but is disabled. Enable it instead of creating a new one.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<ProductivityRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(BonusType.FixedAmount, 50, 100, 500)]
        [InlineData(BonusType.Percentage, 30, 80, 15)]
        public async Task HandleAsync_WithDifferentTypes_ShouldCreateRuleSuccessfully(BonusType type, int minimum, int full, int bonus)
        {
            // Arrange
            var command = new CreateProductivityRuleCommand
            {
                MinimumTarget = minimum,
                FullBonusTarget = full,
                BonusValue = bonus,
                BonusType = type.ToString()
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductivityRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.BonusType.Should().Be(type.ToString());
            result.MinimumTarget.Should().Be(minimum);
            result.FullBonusTarget.Should().Be(full);
            result.BonusValue.Should().Be(bonus);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<ProductivityRule>(r =>
                r.MinimumTarget == minimum &&
                r.FullBonusTarget == full &&
                r.BonusValue == bonus &&
                r.BonusType == type &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
