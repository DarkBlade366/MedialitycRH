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
    public class ApproveTimeEntriesBatchHandlerTests
    {
        private readonly Mock<ITimeEntryRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ApproveTimeEntriesBatchHandler _handler;

        public ApproveTimeEntriesBatchHandlerTests()
        {
            _repositoryMock = new Mock<ITimeEntryRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ApproveTimeEntriesBatchHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WhenEmptyBatch_ShouldReturnEmptyResults()
        {
            // Arrange
            var command = new ApproveTimeEntriesBatchCommand
            {
                Items = new List<ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem>()
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _repositoryMock.Verify(x => x.GetByIdsAsync(It.IsAny<List<Guid>>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAllTimeEntriesFoundAndNotReviewed_ShouldApproveAllSuccessfully()
        {
            // Arrange
            var timeEntry1 = new TimeEntry(
                123,
                456,
                Guid.NewGuid(),
                10.0m,
                DateTime.Now.AddDays(-1),
                10,
                "Development"
            );

            var timeEntry2 = new TimeEntry(
                124,
                457,
                Guid.NewGuid(),
                8.0m,
                DateTime.Now.AddDays(-2),
                11,
                "Testing"
            );

            var timeEntryId1 = timeEntry1.Id;
            var timeEntryId2 = timeEntry2.Id;

            var command = new ApproveTimeEntriesBatchCommand
            {
                Items = new List<ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem>
                {
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId1, ApprovedHours = 8.0m },
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId2, ApprovedHours = 6.5m }
                }
            };

            var timeEntries = new List<TimeEntry> { timeEntry1, timeEntry2 };

            _repositoryMock
                .Setup(x => x.GetByIdsAsync(It.Is<List<Guid>>(ids => ids.SequenceEqual(new[] { timeEntryId1, timeEntryId2 }))))
                .ReturnsAsync(timeEntries);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            // First result
            var result1 = result.First();
            result1.TimeEntryId.Should().Be(timeEntryId1);
            result1.Success.Should().BeTrue();
            result1.Message.Should().MatchRegex(@"Approved 8[.,]0h of 10[.,]0h");
            result1.TimeEntry.Should().NotBeNull();
            result1.TimeEntry!.Id.Should().Be(timeEntry1.Id);
            result1.TimeEntry.RedmineTimeEntryId.Should().Be(123);
            result1.TimeEntry.RedmineProjectId.Should().Be(456);
            result1.TimeEntry.RedmineActivityId.Should().Be(10);
            result1.TimeEntry.ActivityName.Should().Be("Development");
            result1.TimeEntry.Hours.Should().Be(10.0m);
            result1.TimeEntry.ApprovedHours.Should().Be(8.0m);
            result1.TimeEntry.Reviewed.Should().BeTrue();

            // Second result
            var result2 = result.Last();
            result2.TimeEntryId.Should().Be(timeEntryId2);
            result2.Success.Should().BeTrue();
            result2.Message.Should().MatchRegex(@"Approved 6[.,]5h of 8[.,]0h");
            result2.TimeEntry.Should().NotBeNull();
            result2.TimeEntry!.Id.Should().Be(timeEntry2.Id);
            result2.TimeEntry.RedmineTimeEntryId.Should().Be(124);
            result2.TimeEntry.RedmineProjectId.Should().Be(457);
            result2.TimeEntry.RedmineActivityId.Should().Be(11);
            result2.TimeEntry.ActivityName.Should().Be("Testing");
            result2.TimeEntry.Hours.Should().Be(8.0m);
            result2.TimeEntry.ApprovedHours.Should().Be(6.5m);
            result2.TimeEntry.Reviewed.Should().BeTrue();

            timeEntry1.ApprovedHours.Should().Be(8.0m);
            timeEntry1.Reviewed.Should().BeTrue();
            timeEntry2.ApprovedHours.Should().Be(6.5m);
            timeEntry2.Reviewed.Should().BeTrue();

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenSomeTimeEntriesNotFound_ShouldReturnMixedResults()
        {
            // Arrange
            var timeEntry1 = new TimeEntry(
                123,
                456,
                Guid.NewGuid(),
                10.0m,
                DateTime.Now.AddDays(-1),
                10,
                "Development"
            );

            var timeEntry3 = new TimeEntry(
                125,
                458,
                Guid.NewGuid(),
                9.0m,
                DateTime.Now.AddDays(-3),
                12,
                "Documentation"
            );

            var timeEntryId1 = timeEntry1.Id;
            var timeEntryId3 = timeEntry3.Id;
            var timeEntryId2 = Guid.NewGuid();

            var command = new ApproveTimeEntriesBatchCommand
            {
                Items = new List<ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem>
                {
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId1, ApprovedHours = 8.0m },
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId2, ApprovedHours = 6.5m },
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId3, ApprovedHours = 7.0m }
                }
            };

            var timeEntries = new List<TimeEntry> { timeEntry1, timeEntry3 };

            _repositoryMock
                .Setup(x => x.GetByIdsAsync(It.Is<List<Guid>>(ids => ids.SequenceEqual(new[] { timeEntryId1, timeEntryId2, timeEntryId3 }))))
                .ReturnsAsync(timeEntries);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            var result1 = result[0];
            result1.TimeEntryId.Should().Be(timeEntryId1);
            result1.Success.Should().BeTrue();
            result1.Message.Should().MatchRegex(@"Approved 8[.,]0h of 10[.,]0h");
            result1.TimeEntry.Should().NotBeNull();

            var result2 = result[1];
            result2.TimeEntryId.Should().Be(timeEntryId2);
            result2.Success.Should().BeFalse();
            result2.Message.Should().Be("Time entry not found.");
            result2.TimeEntry.Should().BeNull();

            var result3 = result[2];
            result3.TimeEntryId.Should().Be(timeEntryId3);
            result3.Success.Should().BeTrue();
            result3.Message.Should().MatchRegex(@"Approved 7[.,]0h of 9[.,]0h");
            result3.TimeEntry.Should().NotBeNull();

            timeEntry1.ApprovedHours.Should().Be(8.0m);
            timeEntry1.Reviewed.Should().BeTrue();
            timeEntry3.ApprovedHours.Should().Be(7.0m);
            timeEntry3.Reviewed.Should().BeTrue();

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenSomeTimeEntriesAlreadyReviewed_ShouldReturnMixedResults()
        {
            // Arrange
            var timeEntry1 = new TimeEntry(
                123,
                456,
                Guid.NewGuid(),
                10.0m,
                DateTime.Now.AddDays(-1),
                10,
                "Development"
            );

            var timeEntry2 = new TimeEntry(
                124,
                457,
                Guid.NewGuid(),
                8.0m,
                DateTime.Now.AddDays(-2),
                11,
                "Testing"
            );
            timeEntry2.Approve(5.0m);

            var timeEntryId1 = timeEntry1.Id;
            var timeEntryId2 = timeEntry2.Id;

            var command = new ApproveTimeEntriesBatchCommand
            {
                Items = new List<ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem>
                {
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId1, ApprovedHours = 8.0m },
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId2, ApprovedHours = 6.5m }
                }
            };

            var timeEntries = new List<TimeEntry> { timeEntry1, timeEntry2 };

            _repositoryMock
                .Setup(x => x.GetByIdsAsync(It.Is<List<Guid>>(ids => ids.SequenceEqual(new[] { timeEntryId1, timeEntryId2 }))))
                .ReturnsAsync(timeEntries);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var result1 = result[0];
            result1.TimeEntryId.Should().Be(timeEntryId1);
            result1.Success.Should().BeTrue();
            result1.Message.Should().MatchRegex(@"Approved 8[.,]0h of 10[.,]0h");
            result1.TimeEntry.Should().NotBeNull();

            var result2 = result[1];
            result2.TimeEntryId.Should().Be(timeEntryId2);
            result2.Success.Should().BeFalse();
            result2.Message.Should().Be("Time entry already reviewed.");
            result2.TimeEntry.Should().BeNull();

            timeEntry1.ApprovedHours.Should().Be(8.0m);
            timeEntry1.Reviewed.Should().BeTrue();
            timeEntry2.ApprovedHours.Should().Be(5.0m);
            timeEntry2.Reviewed.Should().BeTrue();

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenMixedScenarios_ShouldProcessCorrectly()
        {
            // Arrange
            var timeEntry1 = new TimeEntry(
                123,
                456,
                Guid.NewGuid(),
                10.0m,
                DateTime.Now.AddDays(-1),
                10,
                "Development"
            );

            var timeEntry3 = new TimeEntry(
                125,
                458,
                Guid.NewGuid(),
                9.0m,
                DateTime.Now.AddDays(-3),
                12,
                "Documentation"
            );
            timeEntry3.Approve(5.0m);

            var timeEntry4 = new TimeEntry(
                126,
                459,
                Guid.NewGuid(),
                11.0m,
                DateTime.Now.AddDays(-4),
                13,
                "Research"
            );

            var timeEntryId1 = timeEntry1.Id;
            var timeEntryId2 = Guid.NewGuid(); 
            var timeEntryId3 = timeEntry3.Id;
            var timeEntryId4 = timeEntry4.Id;

            var command = new ApproveTimeEntriesBatchCommand
            {
                Items = new List<ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem>
                {
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId1, ApprovedHours = 8.0m },
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId2, ApprovedHours = 6.5m },
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId3, ApprovedHours = 7.0m },
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId4, ApprovedHours = 9.0m }
                }
            };

            var timeEntries = new List<TimeEntry> { timeEntry1, timeEntry3, timeEntry4 };

            _repositoryMock
                .Setup(x => x.GetByIdsAsync(It.Is<List<Guid>>(ids => ids.SequenceEqual(new[] { timeEntryId1, timeEntryId2, timeEntryId3, timeEntryId4 }))))
                .ReturnsAsync(timeEntries);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(4);

            var results = result.ToList();

            results[0].TimeEntryId.Should().Be(timeEntryId1);
            results[0].Success.Should().BeTrue();
            results[0].Message.Should().MatchRegex(@"Approved 8[.,]0h of 10[.,]0h");
            results[0].TimeEntry.Should().NotBeNull();

            results[1].TimeEntryId.Should().Be(timeEntryId2);
            results[1].Success.Should().BeFalse();
            results[1].Message.Should().Be("Time entry not found.");
            results[1].TimeEntry.Should().BeNull();

            results[2].TimeEntryId.Should().Be(timeEntryId3);
            results[2].Success.Should().BeFalse();
            results[2].Message.Should().Be("Time entry already reviewed.");
            results[2].TimeEntry.Should().BeNull();

            results[3].TimeEntryId.Should().Be(timeEntryId4);
            results[3].Success.Should().BeTrue();
            results[3].Message.Should().MatchRegex(@"Approved 9[.,]0h of 11[.,]0h");
            results[3].TimeEntry.Should().NotBeNull();

            timeEntry1.ApprovedHours.Should().Be(8.0m);
            timeEntry1.Reviewed.Should().BeTrue();
            timeEntry3.ApprovedHours.Should().Be(5.0m);
            timeEntry3.Reviewed.Should().BeTrue();
            timeEntry4.ApprovedHours.Should().Be(9.0m);
            timeEntry4.Reviewed.Should().BeTrue();

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var timeEntryId = Guid.NewGuid();
            var command = new ApproveTimeEntriesBatchCommand
            {
                Items = new List<ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem>
                {
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId, ApprovedHours = 8.0m }
                }
            };

            _repositoryMock
                .Setup(x => x.GetByIdsAsync(new List<Guid> { timeEntryId }))
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
            var timeEntry = new TimeEntry(
                123,
                456,
                Guid.NewGuid(),
                10.0m,
                DateTime.Now.AddDays(-1),
                10,
                "Development"
            );
            var timeEntryId = timeEntry.Id;

            var command = new ApproveTimeEntriesBatchCommand
            {
                Items = new List<ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem>
                {
                    new ApproveTimeEntriesBatchCommand.ApproveTimeEntryItem { TimeEntryId = timeEntryId, ApprovedHours = 8.0m }
                }
            };

            _repositoryMock
                .Setup(x => x.GetByIdsAsync(new List<Guid> { timeEntryId }))
                .ReturnsAsync(new List<TimeEntry> { timeEntry });

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
