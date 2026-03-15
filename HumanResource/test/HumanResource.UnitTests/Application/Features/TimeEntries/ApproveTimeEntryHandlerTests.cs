using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.TimeEntries.Handlers;
using Application.Features.TimeEntries.Commands;
using Application.Features.TimeEntries.DTOs;
using Domain.Features.TimeEntries.Interfaces;
using Domain.Features.TimeEntries.Aggregates;
using Application.Common.Interfaces;

namespace HumanResource.UnitTests.Application.Features.TimeEntries
{
    public class ApproveTimeEntryHandlerTests
    {
        private readonly Mock<ITimeEntryRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ApproveTimeEntryHandler _handler;

        public ApproveTimeEntryHandlerTests()
        {
            _repositoryMock = new Mock<ITimeEntryRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ApproveTimeEntryHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidCommand_ShouldApproveTimeEntrySuccessfully()
        {
            // Arrange
            var timeEntryId = Guid.NewGuid();
            var command = new ApproveTimeEntryCommand
            {
                TimeEntryId = timeEntryId,
                ApprovedHours = 8.0m
            };

            var timeEntry = new TimeEntry(
                123, 
                456, 
                Guid.NewGuid(), 
                10.0m,
                DateTime.Now.AddDays(-1), 
                10, 
                "Development" 
            );

            _repositoryMock
                .Setup(x => x.GetByIdAsync(timeEntryId))
                .ReturnsAsync(timeEntry);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(timeEntry.Id);
            result.RedmineTimeEntryId.Should().Be(123);
            result.RedmineProjectId.Should().Be(456);
            result.RedmineActivityId.Should().Be(10);
            result.ActivityName.Should().Be("Development");
            result.Hours.Should().Be(10.0m);
            result.ApprovedHours.Should().Be(8.0m);
            result.Reviewed.Should().BeTrue();
            timeEntry.ApprovedHours.Should().Be(8.0m);
            timeEntry.Reviewed.Should().BeTrue();
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenTimeEntryNotFound_ShouldThrowException()
        {
            // Arrange
            var timeEntryId = Guid.NewGuid();
            var command = new ApproveTimeEntryCommand
            {
                TimeEntryId = timeEntryId,
                ApprovedHours = 8.0m
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(timeEntryId))
                .ReturnsAsync((TimeEntry?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Be("Time entry not found.");
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenTimeEntryAlreadyReviewed_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var timeEntryId = Guid.NewGuid();
            var command = new ApproveTimeEntryCommand
            {
                TimeEntryId = timeEntryId,
                ApprovedHours = 8.0m
            };

            var timeEntry = new TimeEntry(
                123, 
                456, 
                Guid.NewGuid(),
                10.0m,
                DateTime.Now.AddDays(-1), 
                10,
                "Development" 
            );
            
            timeEntry.Approve(7.5m); 

            _repositoryMock
                .Setup(x => x.GetByIdAsync(timeEntryId))
                .ReturnsAsync(timeEntry);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Be("This time entry has already been approved.");
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(5.0)]
        [InlineData(8.0)]
        public async Task Handle_WhenApprovedHoursAreValid_ShouldStillApprove(decimal approvedHours)
        {
            // Arrange
            var timeEntryId = Guid.NewGuid();
            var command = new ApproveTimeEntryCommand
            {
                TimeEntryId = timeEntryId,
                ApprovedHours = (decimal)approvedHours
            };

            var timeEntry = new TimeEntry(
                123, 
                456, 
                Guid.NewGuid(), 
                10.0m, 
                DateTime.Now.AddDays(-1),
                10, 
                "Development" 
            );

            _repositoryMock
                .Setup(x => x.GetByIdAsync(timeEntryId))
                .ReturnsAsync(timeEntry);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.ApprovedHours.Should().Be((decimal)approvedHours);
            timeEntry.ApprovedHours.Should().Be((decimal)approvedHours);
            timeEntry.Reviewed.Should().BeTrue();
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenApprovedHoursEqualOriginalHours_ShouldApproveSuccessfully()
        {
            // Arrange
            var timeEntryId = Guid.NewGuid();
            var command = new ApproveTimeEntryCommand
            {
                TimeEntryId = timeEntryId,
                ApprovedHours = 10.0m 
            };

            var timeEntry = new TimeEntry(
                123, 
                456, 
                Guid.NewGuid(),
                10.0m, 
                DateTime.Now.AddDays(-1), 
                10, 
                "Development" 
            );

            _repositoryMock
                .Setup(x => x.GetByIdAsync(timeEntryId))
                .ReturnsAsync(timeEntry);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Hours.Should().Be(10.0m);
            result.ApprovedHours.Should().Be(10.0m);
            result.Reviewed.Should().BeTrue();
            timeEntry.ApprovedHours.Should().Be(10.0m);
            timeEntry.Reviewed.Should().BeTrue();
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var timeEntryId = Guid.NewGuid();
            var command = new ApproveTimeEntryCommand
            {
                TimeEntryId = timeEntryId,
                ApprovedHours = 8.0m
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(timeEntryId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Be("Database error");
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUnitOfWorkThrowsException_ShouldPropagateException()
        {
            // Arrange
            var timeEntryId = Guid.NewGuid();
            var command = new ApproveTimeEntryCommand
            {
                TimeEntryId = timeEntryId,
                ApprovedHours = 8.0m
            };

            var timeEntry = new TimeEntry(
                123, 
                456,
                Guid.NewGuid(), 
                10.0m,
                DateTime.Now.AddDays(-1),
                10, 
                "Development"
            );

            _repositoryMock
                .Setup(x => x.GetByIdAsync(timeEntryId))
                .ReturnsAsync(timeEntry);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Save error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            exception.Message.Should().Be("Save error");
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
