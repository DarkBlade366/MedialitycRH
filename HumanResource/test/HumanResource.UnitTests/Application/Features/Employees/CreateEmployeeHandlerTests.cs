using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Employees.Handlers;
using Application.Features.Employees.Commands;
using Application.Common.Interfaces;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Domain.Common.Security;
    
namespace HumanResource.UnitTests.Application.Features.Employees
{
    public class CreateEmployeeHandlerTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly CreateEmployeeHandler _handler;

        public CreateEmployeeHandlerTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheService>();
            _handler = new CreateEmployeeHandler(_employeeRepositoryMock.Object, _unitOfWorkMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidData_ShouldCreateEmployeeAndReturnId()
        {
            // Arrange
            var command = new CreateEmployeeCommand
            {
                FullName = "John Doe",
                Email = "john@example.com",
                Password = "SecurePassword123!",
                RedmineUserId = 123,
                Role = EmployeeRole.Employee
            };

            _employeeRepositoryMock
                .Setup(x => x.ExistsByRedmineUserIdAsync(command.RedmineUserId))
                .ReturnsAsync(false);

            _employeeRepositoryMock
                .Setup(x => x.ExistsByEmailAsync(command.Email.Trim().ToLowerInvariant()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _employeeRepositoryMock.Verify(x => x.AddAsync(It.Is<Employee>(e => 
                e.FullName == "John Doe" &&
                e.Email == "john@example.com" &&
                e.RedmineUserId == 123 &&
                e.Role == EmployeeRole.Employee &&
                e.IsActive)), Times.Once);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRedmineUserIdAlreadyExists_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var command = new CreateEmployeeCommand
            {
                FullName = "John Doe",
                Email = "john@example.com",
                Password = "SecurePassword123!",
                RedmineUserId = 123,
                Role = EmployeeRole.Employee
            };

            _employeeRepositoryMock
                .Setup(x => x.ExistsByRedmineUserIdAsync(command.RedmineUserId))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("An employee with RedmineUserId 123 already exists.");
            _employeeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenEmailAlreadyExists_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var command = new CreateEmployeeCommand
            {
                FullName = "John Doe",
                Email = "john@example.com",
                Password = "SecurePassword123!",
                RedmineUserId = 123,
                Role = EmployeeRole.Employee
            };

            _employeeRepositoryMock
                .Setup(x => x.ExistsByRedmineUserIdAsync(command.RedmineUserId))
                .ReturnsAsync(false);

            _employeeRepositoryMock
                .Setup(x => x.ExistsByEmailAsync(command.Email.Trim().ToLowerInvariant()))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("Email already in use.");
            _employeeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Employee>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenEmailWithDifferentCase_ShouldNormalizeAndCheckDuplicates()
        {
            // Arrange
            var command = new CreateEmployeeCommand
            {
                FullName = "John Doe",
                Email = "JOHN@EXAMPLE.COM",
                Password = "SecurePassword123!",
                RedmineUserId = 123,
                Role = EmployeeRole.Employee
            };

            _employeeRepositoryMock
                .Setup(x => x.ExistsByRedmineUserIdAsync(command.RedmineUserId))
                .ReturnsAsync(false);

            _employeeRepositoryMock
                .Setup(x => x.ExistsByEmailAsync("john@example.com"))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _employeeRepositoryMock.Verify(x => x.AddAsync(It.Is<Employee>(e => 
                e.Email == "john@example.com")), Times.Once); 
        }

        [Fact]
        public async Task Handle_WhenValidData_ShouldCreateEmployeeWithInitialVacationAndAguinaldoBalance()
        {
            // Arrange
            var command = new CreateEmployeeCommand
            {
                FullName = "Jane Smith",
                Email = "jane@example.com",
                Password = "SecurePassword123!",
                RedmineUserId = 456,
                Role = EmployeeRole.ProjectManager
            };

            _employeeRepositoryMock
                .Setup(x => x.ExistsByRedmineUserIdAsync(command.RedmineUserId))
                .ReturnsAsync(false);

            _employeeRepositoryMock
                .Setup(x => x.ExistsByEmailAsync(command.Email.Trim().ToLowerInvariant()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _employeeRepositoryMock.Verify(x => x.AddAsync(It.Is<Employee>(e => 
                e.VacationBalance != null &&
                e.AguinaldoBalance != null &&
                e.VacationBalance.AvailableDays == 0 &&
                e.AguinaldoBalance.AccruedAmount == 0)), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenValidData_ShouldCreateEmployeeAsActiveByDefault()
        {
            // Arrange
            var command = new CreateEmployeeCommand
            {
                FullName = "Bob Wilson",
                Email = "bob@example.com",
                Password = "SecurePassword123!",
                RedmineUserId = 789,
                Role = EmployeeRole.ProjectManager
            };

            _employeeRepositoryMock
                .Setup(x => x.ExistsByRedmineUserIdAsync(command.RedmineUserId))
                .ReturnsAsync(false);

            _employeeRepositoryMock
                .Setup(x => x.ExistsByEmailAsync(command.Email.Trim().ToLowerInvariant()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _employeeRepositoryMock.Verify(x => x.AddAsync(It.Is<Employee>(e => 
                e.IsActive == true)), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenValidData_ShouldHashPasswordBeforeStoring()
        {
            // Arrange
            var command = new CreateEmployeeCommand
            {
                FullName = "Alice Brown",
                Email = "alice@example.com",
                Password = "PlainTextPassword",
                RedmineUserId = 999,
                Role = EmployeeRole.HumanResources
            };

            _employeeRepositoryMock
                .Setup(x => x.ExistsByRedmineUserIdAsync(command.RedmineUserId))
                .ReturnsAsync(false);

            _employeeRepositoryMock
                .Setup(x => x.ExistsByEmailAsync(command.Email.Trim().ToLowerInvariant()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _employeeRepositoryMock.Verify(x => x.AddAsync(It.Is<Employee>(e => 
                e.PasswordHash != "PlainTextPassword" &&
                e.PasswordHash != string.Empty)), Times.Once);
        }
    }
}
