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
    public class ChangeEmployeeStatusHandlerTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ChangeEmployeeStatusHandler _handler;

        public ChangeEmployeeStatusHandlerTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ChangeEmployeeStatusHandler(_employeeRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WhenEmployeeExists_ShouldChangeStatusAndSave()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new ChangeEmployeeStatusCommand { Id = employeeId, IsActive = false };
            
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.ProjectManager,
                "hashedPassword",
                123);
            
            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            employee.IsActive.Should().BeFalse();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEmployeeDoesNotExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new ChangeEmployeeStatusCommand { Id = employeeId, IsActive = true };
            
            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync((Employee?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _handler.Handle(command, CancellationToken.None));
            
            exception.Message.Should().Be("Employee not found");
        }

        [Fact]
        public async Task Handle_WhenStatusIsAlreadySame_ShouldNotChangeStatusButStillSave()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new ChangeEmployeeStatusCommand { Id = employeeId, IsActive = true };
            
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.ProjectManager,
                "hashedPassword",
                123);
            
            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            employee.IsActive.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenActivatingInactiveEmployee_ShouldChangeToActive()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new ChangeEmployeeStatusCommand { Id = employeeId, IsActive = true };
            
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.ProjectManager,
                "hashedPassword",
                123);
    
            employee.ChangeStatus(false);
            employee.IsActive.Should().BeFalse();
            
            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            employee.IsActive.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenDeactivatingActiveEmployee_ShouldChangeToInactive()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var command = new ChangeEmployeeStatusCommand { Id = employeeId, IsActive = false };
            
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.ProjectManager,
                "hashedPassword",
                123);
            
            employee.IsActive.Should().BeTrue();
            
            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            employee.IsActive.Should().BeFalse();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
