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
using Domain.Features.Employees.Entities;
using Application.Common.Interfaces;
using Domain.Common.Security;

namespace HumanResource.UnitTests.Application.Features.Redmine
{
    public class SyncRedmineUsersHandlerTests
    {
        private readonly Mock<IRedmineService> _redmineServiceMock;
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly SyncRedmineUsersHandler _handler;

        public SyncRedmineUsersHandlerTests()
        {
            _redmineServiceMock = new Mock<IRedmineService>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new SyncRedmineUsersHandler(
                _redmineServiceMock.Object,
                _employeeRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WhenNoRedmineUsers_ShouldReturnZero()
        {
            // Arrange
            _redmineServiceMock
                .Setup(x => x.GetUsersAsync())
                .ReturnsAsync(new List<RedmineUserDto>());

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _employeeRepositoryMock.Verify(x => x.GetAllActiveAsync(), Times.Never);
            _employeeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<Employee>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenNewUsersOnly_ShouldCreateAllUsers()
        {
            // Arrange
            var redmineUsers = new List<RedmineUserDto>
            {
                new RedmineUserDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new RedmineUserDto { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
            };

            var existingEmployees = new List<Employee>();

            _redmineServiceMock
                .Setup(x => x.GetUsersAsync())
                .ReturnsAsync(redmineUsers);

            _employeeRepositoryMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(existingEmployees);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(2);
            _employeeRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<Employee>>(employees =>
                employees.Count == 2 &&
                employees.Any(e => e.FullName == "John Doe" && e.Email == "john@example.com" && e.RedmineUserId == 1) &&
                employees.Any(e => e.FullName == "Jane Smith" && e.Email == "jane@example.com" && e.RedmineUserId == 2))), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenExistingUsersOnly_ShouldUpdateChangedUsers()
        {
            // Arrange
            var redmineUsers = new List<RedmineUserDto>
            {
                new RedmineUserDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new RedmineUserDto { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" } 
            };

            var existingEmployees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1),
                new Employee("Jane Smith", "jane@example.com", EmployeeRole.Employee, "hashed", 2)
            };

            _redmineServiceMock
                .Setup(x => x.GetUsersAsync())
                .ReturnsAsync(redmineUsers);

            _employeeRepositoryMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(existingEmployees);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(0);
            _employeeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<Employee>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenMixedUsers_ShouldCreateNewAndUpdateExisting()
        {
            // Arrange
            var redmineUsers = new List<RedmineUserDto>
            {
                new RedmineUserDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new RedmineUserDto { Id = 3, FirstName = "Bob", LastName = "Wilson", Email = "bob@example.com" } 
            };

            var existingEmployees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1)
            };

            _redmineServiceMock
                .Setup(x => x.GetUsersAsync())
                .ReturnsAsync(redmineUsers);

            _employeeRepositoryMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(existingEmployees);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(1);
            _employeeRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<Employee>>(employees =>
                employees.Count == 1 &&
                employees.Any(e => e.FullName == "Bob Wilson" && e.Email == "bob@example.com" && e.RedmineUserId == 3))), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUsersWithMissingEmail_ShouldSkipThem()
        {
            // Arrange
            var redmineUsers = new List<RedmineUserDto>
            {
                new RedmineUserDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new RedmineUserDto { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "" },
                new RedmineUserDto { Id = 3, FirstName = "Bob", LastName = "Wilson", Email = (string?)null }, 
                new RedmineUserDto { Id = 4, FirstName = "Alice", LastName = "Brown", Email = "alice@example.com" }
            };

            var existingEmployees = new List<Employee>();

            _redmineServiceMock
                .Setup(x => x.GetUsersAsync())
                .ReturnsAsync(redmineUsers);

            _employeeRepositoryMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(existingEmployees);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(2); 
            _employeeRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<List<Employee>>(employees =>
                employees.Count == 2 &&
                employees.Any(e => e.FullName == "John Doe" && e.RedmineUserId == 1) &&
                employees.Any(e => e.FullName == "Alice Brown" && e.RedmineUserId == 4) &&
                !employees.Any(e => e.RedmineUserId == 2) &&
                !employees.Any(e => e.RedmineUserId == 3))), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUsersNoLongerInRedmine_ShouldDeactivateNonAdmins()
        {
            // Arrange
            var redmineUsers = new List<RedmineUserDto>
            {
                new RedmineUserDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" }
            };

            var existingEmployees = new List<Employee>
            {
                new Employee("John Doe", "john@example.com", EmployeeRole.Employee, "hashed", 1),
                new Employee("Jane Smith", "jane@example.com", EmployeeRole.Employee, "hashed", 2),
                new Employee("Admin User", "admin@example.com", EmployeeRole.Administrator, "hashed", 3) 
            };

            _redmineServiceMock
                .Setup(x => x.GetUsersAsync())
                .ReturnsAsync(redmineUsers);

            _employeeRepositoryMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(existingEmployees);

            // Act
            var result = await _handler.Handle(CancellationToken.None);

            // Assert
            result.Should().Be(0);
            
            var deactivatedEmployees = existingEmployees.Where(e => !e.IsActive).ToList();
            deactivatedEmployees.Should().ContainSingle(e => e.FullName == "Jane Smith");
            deactivatedEmployees.Should().NotContain(e => e.FullName == "Admin User");
            
            _employeeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<Employee>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRedmineServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            _redmineServiceMock
                .Setup(x => x.GetUsersAsync())
                .ThrowsAsync(new Exception("Redmine API error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(CancellationToken.None));
            exception.Message.Should().Be("Redmine API error");
            
            _employeeRepositoryMock.Verify(x => x.GetAllActiveAsync(), Times.Never);
            _employeeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<Employee>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var redmineUsers = new List<RedmineUserDto>
            {
                new RedmineUserDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" }
            };

            _redmineServiceMock
                .Setup(x => x.GetUsersAsync())
                .ReturnsAsync(redmineUsers);

            _employeeRepositoryMock
                .Setup(x => x.GetAllActiveAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(CancellationToken.None));
            exception.Message.Should().Be("Database error");
            
            _employeeRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<Employee>>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
