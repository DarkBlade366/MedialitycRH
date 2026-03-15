using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Redmine.Handlers;
using Application.Features.Redmine.Interfaces;
using Application.Features.Redmine.DTOs;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.TimeEntries.Aggregates;
using Domain.Features.TimeEntries.Interfaces;
using Application.Common.Interfaces;

namespace HumanResource.UnitTests.Application.Features.Redmine
{
    public class SyncRedmineTimeEntriesHandlerTests
    {
        private readonly Mock<IRedmineService> _redmineServiceMock;
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<ITimeEntryRepository> _timeRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly SyncRedmineTimeEntriesHandler _handler;

        public SyncRedmineTimeEntriesHandlerTests()
        {
            _redmineServiceMock = new Mock<IRedmineService>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _timeRepositoryMock = new Mock<ITimeEntryRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new SyncRedmineTimeEntriesHandler(
                _redmineServiceMock.Object,
                _employeeRepositoryMock.Object,
                _timeRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WhenNoActiveEmployees_ShouldReturnZero()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>();
            var pagedResult = (employees, 0);

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _handler.Handle(from, to, CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _redmineServiceMock.Verify(x => x.GetTimeEntriesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int?>()), Times.Never);
            _timeRepositoryMock.Verify(x => x.GetByRedmineIdsAsync(It.IsAny<List<int>>()), Times.Never);
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<TimeEntry>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEmployeesWithNoRedmineUserId_ShouldSkipThem()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1),
                new Employee("Jane Smith", "jane@example.com", EmployeeRole.Employee, "hashed", 0),
                new Employee("Bob Wilson", "bob@example.com", EmployeeRole.Employee, "hashed", -1) 
            };
            var pagedResult = (employees, 3);

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 1))
                .ReturnsAsync(new List<RedmineTimeEntryDto>());

            // Act
            var result = await _handler.Handle(from, to, CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _redmineServiceMock.Verify(x => x.GetTimeEntriesAsync(from, to, 1), Times.Once);
            _redmineServiceMock.Verify(x => x.GetTimeEntriesAsync(from, to, 0), Times.Never);
            _redmineServiceMock.Verify(x => x.GetTimeEntriesAsync(from, to, -1), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEmployeesWithNoTimeEntries_ShouldReturnZero()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1)
            };
            var pagedResult = (employees, 1);

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 1))
                .ReturnsAsync(new List<RedmineTimeEntryDto>());

            // Act
            var result = await _handler.Handle(from, to, CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _timeRepositoryMock.Verify(x => x.GetByRedmineIdsAsync(It.IsAny<List<int>>()), Times.Never);
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<TimeEntry>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNewTimeEntriesOnly_ShouldCreateAllEntries()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1)
            };
            var pagedResult = (employees, 1);

            var timeEntries = new List<RedmineTimeEntryDto>
            {
                new RedmineTimeEntryDto
                {
                    Id = 1,
                    Hours = 8.5m,
                    SpentOn = DateTime.Now.AddDays(-1),
                    Project = new RedmineProjectReference { Id = 123 },
                    Activity = new RedmineActivityReference { Id = 10, Name = "Development" }
                },
                new RedmineTimeEntryDto
                {
                    Id = 2,
                    Hours = 4.0m,
                    SpentOn = DateTime.Now.AddDays(-2),
                    Project = new RedmineProjectReference { Id = 123 },
                    Activity = new RedmineActivityReference { Id = 11, Name = "Testing" }
                }
            };

            var existingEntries = new List<TimeEntry>();

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 1))
                .ReturnsAsync(timeEntries);

            _timeRepositoryMock
                .Setup(x => x.GetByRedmineIdsAsync(new List<int> { 1, 2 }))
                .ReturnsAsync(existingEntries);

            // Act
            var result = await _handler.Handle(from, to, CancellationToken.None);

            // Assert
            result.Should().Be(2);
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<TimeEntry>>(entries =>
                entries.Count == 2 &&
                entries.Any(e => e.RedmineTimeEntryId == 1 && e.Hours == 8.5m && e.RedmineProjectId == 123 && e.EmployeeId == employees.First().Id && e.RedmineActivityId == 10 && e.ActivityName == "Development") &&
                entries.Any(e => e.RedmineTimeEntryId == 2 && e.Hours == 4.0m && e.RedmineProjectId == 123 && e.EmployeeId == employees.First().Id && e.RedmineActivityId == 11 && e.ActivityName == "Testing"))), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenExistingTimeEntriesOnly_ShouldUpdateExistingEntries()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1)
            };
            var pagedResult = (employees, 1);

            var timeEntries = new List<RedmineTimeEntryDto>
            {
                new RedmineTimeEntryDto
                {
                    Id = 1,
                    Hours = 10.0m,
                    SpentOn = DateTime.Now.AddDays(-1),
                    Project = new RedmineProjectReference { Id = 123 },
                    Activity = new RedmineActivityReference { Id = 10, Name = "Development" }
                }
            };

            var existingEntries = new List<TimeEntry>
            {
                new TimeEntry(1, 123, employees.First().Id, 8.5m, DateTime.Now.AddDays(-1), 10, "Development")
            };

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 1))
                .ReturnsAsync(timeEntries);

            _timeRepositoryMock
                .Setup(x => x.GetByRedmineIdsAsync(new List<int> { 1 }))
                .ReturnsAsync(existingEntries);

            // Act
            var result = await _handler.Handle(from, to, CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<TimeEntry>>()), Times.Never);
            
            var updatedEntry = existingEntries.First();
            updatedEntry.Hours.Should().Be(10.0m);
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenMixedTimeEntries_ShouldCreateNewAndUpdateExisting()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1)
            };
            var pagedResult = (employees, 1);

            var timeEntries = new List<RedmineTimeEntryDto>
            {
                new RedmineTimeEntryDto
                {
                    Id = 1,
                    Hours = 8.5m,
                    SpentOn = DateTime.Now.AddDays(-1),
                    Project = new RedmineProjectReference { Id = 123 },
                    Activity = new RedmineActivityReference { Id = 10, Name = "Development" }
                },
                new RedmineTimeEntryDto
                {
                    Id = 3,
                    Hours = 4.0m,
                    SpentOn = DateTime.Now.AddDays(-2),
                    Project = new RedmineProjectReference { Id = 123 },
                    Activity = new RedmineActivityReference { Id = 11, Name = "Testing" }
                }
            };

            var existingEntries = new List<TimeEntry>
            {
                new TimeEntry(1, 123, employees.First().Id, 8.5m, DateTime.Now.AddDays(-1), 10, "Development")
            };

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 1))
                .ReturnsAsync(timeEntries);

            _timeRepositoryMock
                .Setup(x => x.GetByRedmineIdsAsync(new List<int> { 1, 3 }))
                .ReturnsAsync(existingEntries);

            // Act
            var result = await _handler.Handle(from, to, CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<TimeEntry>>(entries =>
                entries.Count == 1 &&
                entries.Any(e => e.RedmineTimeEntryId == 3 && e.Hours == 4.0m && e.RedmineProjectId == 123 && e.EmployeeId == employees.First().Id && e.RedmineActivityId == 11 && e.ActivityName == "Testing"))), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithMultipleEmployees_ShouldProcessAllEmployees()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1),
                new Employee("Jane Smith", "jane@example.com", EmployeeRole.Employee, "hashed", 2)
            };
            var pagedResult = (employees, 2);

            var employee1Entries = new List<RedmineTimeEntryDto>
            {
                new RedmineTimeEntryDto
                {
                    Id = 1,
                    Hours = 8.5m,
                    SpentOn = DateTime.Now.AddDays(-1),
                    Project = new RedmineProjectReference { Id = 123 },
                    Activity = new RedmineActivityReference { Id = 10, Name = "Development" }
                }
            };

            var employee2Entries = new List<RedmineTimeEntryDto>
            {
                new RedmineTimeEntryDto
                {
                    Id = 2,
                    Hours = 6.0m,
                    SpentOn = DateTime.Now.AddDays(-1),
                    Project = new RedmineProjectReference { Id = 456 },
                    Activity = new RedmineActivityReference { Id = 11, Name = "Testing" }
                },
                new RedmineTimeEntryDto
                {
                    Id = 3,
                    Hours = 4.0m,
                    SpentOn = DateTime.Now.AddDays(-2),
                    Project = new RedmineProjectReference { Id = 456 },
                    Activity = new RedmineActivityReference { Id = 12, Name = "Documentation" }
                }
            };

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 1))
                .ReturnsAsync(employee1Entries);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 2))
                .ReturnsAsync(employee2Entries);

            _timeRepositoryMock
                .Setup(x => x.GetByRedmineIdsAsync(new List<int> { 1 }))
                .ReturnsAsync(new List<TimeEntry>());

            _timeRepositoryMock
                .Setup(x => x.GetByRedmineIdsAsync(new List<int> { 2, 3 }))
                .ReturnsAsync(new List<TimeEntry>());

            // Act
            var result = await _handler.Handle(from, to, CancellationToken.None);

            // Assert
            result.Should().Be(3); 
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<TimeEntry>>(entries =>
                entries.Count == 1 &&
                entries.Any(e => e.RedmineTimeEntryId == 1 && e.RedmineProjectId == 123 && e.EmployeeId == employees[0].Id && e.RedmineActivityId == 10))), Times.Once);
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<TimeEntry>>(entries =>
                entries.Count == 2 &&
                entries.Any(e => e.RedmineTimeEntryId == 2 && e.RedmineProjectId == 456 && e.EmployeeId == employees[1].Id && e.RedmineActivityId == 11) &&
                entries.Any(e => e.RedmineTimeEntryId == 3 && e.RedmineProjectId == 456 && e.EmployeeId == employees[1].Id && e.RedmineActivityId == 12))), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullActivity_ShouldHandleGracefully()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1)
            };
            var pagedResult = (employees, 1);

            var timeEntries = new List<RedmineTimeEntryDto>
            {
                new RedmineTimeEntryDto
                {
                    Id = 1,
                    Hours = 8.5m,
                    SpentOn = DateTime.Now.AddDays(-1),
                    Project = new RedmineProjectReference { Id = 123 },
                    Activity = (RedmineActivityReference?)null 
                }
            };

            var existingEntries = new List<TimeEntry>();

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 1))
                .ReturnsAsync(timeEntries);

            _timeRepositoryMock
                .Setup(x => x.GetByRedmineIdsAsync(new List<int> { 1 }))
                .ReturnsAsync(existingEntries);

            // Act
            var result = await _handler.Handle(from, to, CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<TimeEntry>>(entries =>
                entries.Count == 1 &&
                entries.Any(e => e.RedmineTimeEntryId == 1 && e.Hours == 8.5m && e.RedmineProjectId == 123 && e.EmployeeId == employees.First().Id && e.RedmineActivityId == null && e.ActivityName == null))), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyActivityName_ShouldHandleGracefully()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1)
            };
            var pagedResult = (employees, 1);

            var timeEntries = new List<RedmineTimeEntryDto>
            {
                new RedmineTimeEntryDto
                {
                    Id = 1,
                    Hours = 8.5m,
                    SpentOn = DateTime.Now.AddDays(-1),
                    Project = new RedmineProjectReference { Id = 123 },
                    Activity = new RedmineActivityReference { Id = 10, Name = "" }
                }
            };

            var existingEntries = new List<TimeEntry>();

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 1))
                .ReturnsAsync(timeEntries);

            _timeRepositoryMock
                .Setup(x => x.GetByRedmineIdsAsync(new List<int> { 1 }))
                .ReturnsAsync(existingEntries);

            // Act
            var result = await _handler.Handle(from, to, CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<TimeEntry>>(entries =>
                entries.Count == 1 &&
                entries.Any(e => e.RedmineTimeEntryId == 1 && e.Hours == 8.5m && e.RedmineProjectId == 123 && e.EmployeeId == employees.First().Id && e.RedmineActivityId == 10 && e.ActivityName == null))), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRedmineServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1)
            };
            var pagedResult = (employees, 1);

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 1))
                .ThrowsAsync(new Exception("Redmine API error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(from, to, CancellationToken.None));
            exception.Message.Should().Be("Redmine API error");
            
            _timeRepositoryMock.Verify(x => x.GetByRedmineIdsAsync(It.IsAny<List<int>>()), Times.Never);
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<TimeEntry>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenEmployeeRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(from, to, CancellationToken.None));
            exception.Message.Should().Be("Database error");
            
            _redmineServiceMock.Verify(x => x.GetTimeEntriesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int?>()), Times.Never);
            _timeRepositoryMock.Verify(x => x.GetByRedmineIdsAsync(It.IsAny<List<int>>()), Times.Never);
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<TimeEntry>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenTimeRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1)
            };
            var pagedResult = (employees, 1);

            var timeEntries = new List<RedmineTimeEntryDto>
            {
                new RedmineTimeEntryDto
                {
                    Id = 1,
                    Hours = 8.5m,
                    SpentOn = DateTime.Now.AddDays(-1),
                    Project = new RedmineProjectReference { Id = 123 },
                    Activity = new RedmineActivityReference { Id = 10, Name = "Development" }
                }
            };

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 1))
                .ReturnsAsync(timeEntries);

            _timeRepositoryMock
                .Setup(x => x.GetByRedmineIdsAsync(new List<int> { 1 }))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(from, to, CancellationToken.None));
            exception.Message.Should().Be("Database error");
            
            _timeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<TimeEntry>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUnitOfWorkThrowsException_ShouldPropagateException()
        {
            // Arrange
            var from = DateTime.Now.AddDays(-7);
            var to = DateTime.Now;
            var employees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1)
            };
            var pagedResult = (employees, 1);

            _employeeRepositoryMock
                .Setup(x => x.GetPagedAsync(1, int.MaxValue))
                .ReturnsAsync(pagedResult);

            _redmineServiceMock
                .Setup(x => x.GetTimeEntriesAsync(from, to, 1))
                .ReturnsAsync(new List<RedmineTimeEntryDto>());

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Save error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(from, to, CancellationToken.None));
            exception.Message.Should().Be("Save error");
            
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
