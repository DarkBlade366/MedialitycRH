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

namespace HumanResource.UnitTests.Application.Features.Employees
{
    public class UseVacationHandlerTests
    {
        private readonly Mock<IEmployeeRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly UseVacationHandler _handler;

        public UseVacationHandlerTests()
        {
            _repositoryMock = new Mock<IEmployeeRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheService>();
            _handler = new UseVacationHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_WhenEmployeeExistsAndHasEnoughBalance_ShouldUseVacationDays()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new UseVacationCommand { EmployeeId = employeeId, Days = 5m };
            
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);
            
            employee.AccrueVacationDays(10m);
            employee.VacationBalance.AvailableDays.Should().Be(10m);
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            employee.VacationBalance.UsedDays.Should().Be(5m);
            employee.VacationBalance.AvailableDays.Should().Be(5m);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEmployeeDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new UseVacationCommand { EmployeeId = employeeId, Days = 5m };
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync((Employee?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(command, CancellationToken.None));
            
            exception.Message.Should().Be("Employee not found.");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenNotEnoughVacationBalance_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new UseVacationCommand { EmployeeId = employeeId, Days = 10m };
            
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);
            
            employee.AccrueVacationDays(3m);
            employee.VacationBalance.AvailableDays.Should().Be(3m);
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
            
            exception.Message.Should().Be("Not enough vacation balance.");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenZeroDays_ShouldThrowArgumentException()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new UseVacationCommand { EmployeeId = employeeId, Days = 0m };
            
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);
            
            employee.AccrueVacationDays(5m);
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(command, CancellationToken.None));
            
            exception.Message.Should().Be("Days must be positive.");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenNegativeDays_ShouldThrowArgumentException()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new UseVacationCommand { EmployeeId = employeeId, Days = -2m };
            
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);
            
            employee.AccrueVacationDays(5m);
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _handler.Handle(command, CancellationToken.None));
            
            exception.Message.Should().Be("Days must be positive.");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUsingExactlyAvailableDays_ShouldUseAllAvailableDays()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new UseVacationCommand { EmployeeId = employeeId, Days = 7.5m };
            
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);
            
            employee.AccrueVacationDays(7.5m);
            employee.VacationBalance.AvailableDays.Should().Be(7.5m);
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            employee.VacationBalance.UsedDays.Should().Be(7.5m);
            employee.VacationBalance.AvailableDays.Should().Be(0m);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenMultipleUses_ShouldAccumulateCorrectly()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);
            
            employee.AccrueVacationDays(10m);
            employee.VacationBalance.AvailableDays.Should().Be(10m);
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            var command1 = new UseVacationCommand { EmployeeId = employeeId, Days = 3m };
            var command2 = new UseVacationCommand { EmployeeId = employeeId, Days = 2.5m };
            var command3 = new UseVacationCommand { EmployeeId = employeeId, Days = 4m };

            await _handler.Handle(command1, CancellationToken.None);
            await _handler.Handle(command2, CancellationToken.None);
            await _handler.Handle(command3, CancellationToken.None);

            // Assert
            employee.VacationBalance.UsedDays.Should().Be(9.5m); 
            employee.VacationBalance.AvailableDays.Should().Be(0.5m);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact]
        public async Task Handle_WhenUsingFractionalDays_ShouldWorkCorrectly()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new UseVacationCommand { EmployeeId = employeeId, Days = 1.5m };
            
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);
            
            employee.AccrueVacationDays(2.5m);
            employee.VacationBalance.AvailableDays.Should().Be(2.5m);
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            employee.VacationBalance.UsedDays.Should().Be(1.5m);
            employee.VacationBalance.AvailableDays.Should().Be(1m); 
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
