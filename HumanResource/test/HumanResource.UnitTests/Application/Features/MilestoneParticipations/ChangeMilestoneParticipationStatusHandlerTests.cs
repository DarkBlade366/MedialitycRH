using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Projects.Handlers;
using Application.Features.Projects.Commands;
using Application.Common.Interfaces;
using Domain.Features.Projects.Interfaces;
using Domain.Features.Projects.Aggregates;

namespace HumanResource.UnitTests.Application.Features.MilestoneParticipations
{
    public class ChangeMilestoneParticipationStatusHandlerTests
    {
        private readonly Mock<IMilestoneParticipationRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ChangeMilestoneParticipationStatusHandler _handler;

        public ChangeMilestoneParticipationStatusHandlerTests()
        {
            _repositoryMock = new Mock<IMilestoneParticipationRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ChangeMilestoneParticipationStatusHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenParticipationExistsAndIsNotPaid_ShouldActivateSuccessfully()
        {
            // Arrange
            var participationId = Guid.NewGuid();
            var command = new ChangeMilestoneParticipationStatusCommand { Id = participationId, IsActive = true };
            
            var milestone = new ProjectMilestone(123, "Test Milestone");
            var participation = new MilestoneParticipation(Guid.NewGuid(), Guid.NewGuid(), milestone);
            participation.Deactivate(); // Start as inactive
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(participationId))
                .ReturnsAsync(participation);

            // Act
            await _handler.HandleAsync(command);

            // Assert
            participation.IsActive.Should().BeTrue();
            _repositoryMock.Verify(x => x.Update(participation), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenParticipationExistsAndIsNotPaid_ShouldDeactivateSuccessfully()
        {
            // Arrange
            var participationId = Guid.NewGuid();
            var command = new ChangeMilestoneParticipationStatusCommand { Id = participationId, IsActive = false };
            
            var milestone = new ProjectMilestone(123, "Test Milestone");
            var participation = new MilestoneParticipation(Guid.NewGuid(), Guid.NewGuid(), milestone);
            // Start as active by default
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(participationId))
                .ReturnsAsync(participation);

            // Act
            await _handler.HandleAsync(command);

            // Assert
            participation.IsActive.Should().BeFalse();
            _repositoryMock.Verify(x => x.Update(participation), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenParticipationDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var participationId = Guid.NewGuid();
            var command = new ChangeMilestoneParticipationStatusCommand { Id = participationId, IsActive = true };
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(participationId))
                .ReturnsAsync((MilestoneParticipation?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));
            
            exception.Message.Should().Be("Participation not found.");
            _repositoryMock.Verify(x => x.Update(It.IsAny<MilestoneParticipation>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenParticipationIsAlreadyPaid_ShouldThrowException()
        {
            // Arrange
            var participationId = Guid.NewGuid();
            var command = new ChangeMilestoneParticipationStatusCommand { Id = participationId, IsActive = true };
            
            var milestone = new ProjectMilestone(123, "Test Milestone");
            var participation = new MilestoneParticipation(Guid.NewGuid(), Guid.NewGuid(), milestone);
            participation.MarkAsPaid(); // Mark as paid
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(participationId))
                .ReturnsAsync(participation);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));
            
            exception.Message.Should().Be("Cannot change status of a participation that has already been paid.");
            _repositoryMock.Verify(x => x.Update(It.IsAny<MilestoneParticipation>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingAlreadyActiveParticipation_ShouldThrowException()
        {
            // Arrange
            var participationId = Guid.NewGuid();
            var command = new ChangeMilestoneParticipationStatusCommand { Id = participationId, IsActive = true };
            
            var milestone = new ProjectMilestone(123, "Test Milestone");
            var participation = new MilestoneParticipation(Guid.NewGuid(), Guid.NewGuid(), milestone);
            // Start as active by default
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(participationId))
                .ReturnsAsync(participation);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));
            
            exception.Message.Should().Be("The participant is already active.");
            _repositoryMock.Verify(x => x.Update(It.IsAny<MilestoneParticipation>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDeactivatingAlreadyInactiveParticipation_ShouldThrowException()
        {
            // Arrange
            var participationId = Guid.NewGuid();
            var command = new ChangeMilestoneParticipationStatusCommand { Id = participationId, IsActive = false };
            
            var milestone = new ProjectMilestone(123, "Test Milestone");
            var participation = new MilestoneParticipation(Guid.NewGuid(), Guid.NewGuid(), milestone);
            participation.Deactivate(); // Start as inactive
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(participationId))
                .ReturnsAsync(participation);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));
            
            exception.Message.Should().Be("The participant is already inactive.");
            _repositoryMock.Verify(x => x.Update(It.IsAny<MilestoneParticipation>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenActivatingInactiveParticipation_ShouldCallRepositoryAndUnitOfWork()
        {
            // Arrange
            var participationId = Guid.NewGuid();
            var command = new ChangeMilestoneParticipationStatusCommand { Id = participationId, IsActive = true };
            
            var milestone = new ProjectMilestone(123, "Test Milestone");
            var participation = new MilestoneParticipation(Guid.NewGuid(), Guid.NewGuid(), milestone);
            participation.Deactivate(); // Start as inactive
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(participationId))
                .ReturnsAsync(participation);

            // Act
            await _handler.HandleAsync(command);

            // Assert
            _repositoryMock.Verify(x => x.GetByIdAsync(participationId), Times.Once);
            _repositoryMock.Verify(x => x.Update(participation), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
    }
}
