using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Rules.Milestones.Handlers;
using Application.Features.Payrolls.Rules.Milestones.Commands;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Projects.Interfaces;

namespace Application.Features.Payrolls.Rules
{
    public class CreateMilestoneRuleHandlerTests
    {
        private readonly Mock<IMilestoneRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<IProjectMilestoneRepository> _projectMilestoneRepositoryMock;
        private readonly CreateMilestoneRuleHandler _handler;

        public CreateMilestoneRuleHandlerTests()
        {
            _repositoryMock = new Mock<IMilestoneRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectMilestoneRepositoryMock = new Mock<IProjectMilestoneRepository>();
            _handler = new CreateMilestoneRuleHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object,
                _projectRepositoryMock.Object,
                _projectMilestoneRepositoryMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenValidData_ShouldCreateRuleSuccessfully()
        {
            // Arrange
            var command = new CreateMilestoneRuleCommand
            {
                RedmineProjectId = 123,
                MilestoneName = "Phase 1",
                BonusAmount = 2000m
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(true);

            _projectMilestoneRepositoryMock
                .Setup(x => x.ExistsAsync(123, "Phase 1"))
                .ReturnsAsync(true);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.RedmineProjectId.Should().Be(123);
            result.MilestoneName.Should().Be("Phase 1");
            result.BonusAmount.Should().Be(2000m);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<MilestoneRule>(r =>
                r.RedmineProjectId == 123 &&
                r.MilestoneName == "Phase 1" &&
                r.BonusAmount == 2000m &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenProjectDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var command = new CreateMilestoneRuleCommand
            {
                RedmineProjectId = 123,
                MilestoneName = "Phase 1",
                BonusAmount = 2000m
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Project with Id 123 does not exist.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<MilestoneRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenMilestoneDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var command = new CreateMilestoneRuleCommand
            {
                RedmineProjectId = 123,
                MilestoneName = "Phase 1",
                BonusAmount = 2000m
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(true);

            _projectMilestoneRepositoryMock
                .Setup(x => x.ExistsAsync(123, "Phase 1"))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Milestone 'Phase 1' does not exist in project 123.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<MilestoneRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenActiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateMilestoneRuleCommand
            {
                RedmineProjectId = 123,
                MilestoneName = "Phase 1",
                BonusAmount = 2000m
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(true);

            _projectMilestoneRepositoryMock
                .Setup(x => x.ExistsAsync(123, "Phase 1"))
                .ReturnsAsync(true);

            var existingActiveRule = new MilestoneRule(123, "Phase 1", 1500m);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule> { existingActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Contain("There is already an active milestone rule for project 123 and milestone 'Phase 1' with bonus");
            exception.Message.Should().Contain("disable it before creating a different one.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<MilestoneRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenIdenticalActiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateMilestoneRuleCommand
            {
                RedmineProjectId = 123,
                MilestoneName = "Phase 1",
                BonusAmount = 2000m
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(true);

            _projectMilestoneRepositoryMock
                .Setup(x => x.ExistsAsync(123, "Phase 1"))
                .ReturnsAsync(true);

            var existingIdenticalRule = new MilestoneRule(123, "Phase 1", 2000m);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule> { existingIdenticalRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("A milestone rule for project 123 and milestone 'Phase 1' already exists and is active.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<MilestoneRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenIdenticalInactiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateMilestoneRuleCommand
            {
                RedmineProjectId = 123,
                MilestoneName = "Phase 1",
                BonusAmount = 2000m
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(true);

            _projectMilestoneRepositoryMock
                .Setup(x => x.ExistsAsync(123, "Phase 1"))
                .ReturnsAsync(true);

            var existingInactiveRule = new MilestoneRule(123, "Phase 1", 2000m);
            existingInactiveRule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule> { existingInactiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("A milestone rule for project 123 and milestone Phase 1 with bonus 2000 already exists but is disabled. Enable it instead of creating a new one.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<MilestoneRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDifferentRulesExist_ShouldCreateNewRuleSuccessfully()
        {
            // Arrange
            var command = new CreateMilestoneRuleCommand
            {
                RedmineProjectId = 123,
                MilestoneName = "Phase 2",
                BonusAmount = 3000m
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(true);

            _projectMilestoneRepositoryMock
                .Setup(x => x.ExistsAsync(123, "Phase 2"))
                .ReturnsAsync(true);

            var existingDifferentRule = new MilestoneRule(123, "Phase 1", 2000m);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule> { existingDifferentRule });

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.RedmineProjectId.Should().Be(123);
            result.MilestoneName.Should().Be("Phase 2");
            result.BonusAmount.Should().Be(3000m);

            _repositoryMock.Verify(x => x.AddAsync(It.Is<MilestoneRule>(r =>
                r.RedmineProjectId == 123 &&
                r.MilestoneName == "Phase 2" &&
                r.BonusAmount == 3000m &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(5000)]
        [InlineData(10000)]
        public async Task HandleAsync_WithDifferentBonusAmounts_ShouldCreateRuleSuccessfully(int bonusAmount)
        {
            // Arrange
            var command = new CreateMilestoneRuleCommand
            {
                RedmineProjectId = 123,
                MilestoneName = "Phase 1",
                BonusAmount = (decimal)bonusAmount
            };

            _projectRepositoryMock
                .Setup(x => x.ExistsAsync(123))
                .ReturnsAsync(true);

            _projectMilestoneRepositoryMock
                .Setup(x => x.ExistsAsync(123, "Phase 1"))
                .ReturnsAsync(true);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.BonusAmount.Should().Be((decimal)bonusAmount);

            _repositoryMock.Verify(x => x.AddAsync(It.Is<MilestoneRule>(r =>
                r.RedmineProjectId == 123 &&
                r.MilestoneName == "Phase 1" &&
                r.BonusAmount == (decimal)bonusAmount &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
