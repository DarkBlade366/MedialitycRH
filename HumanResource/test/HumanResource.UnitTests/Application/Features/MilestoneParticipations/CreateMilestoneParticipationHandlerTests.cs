using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Projects.Handlers;
using Application.Features.Projects.Commands;
using Application.Features.Projects.DTOs;
using Application.Common.Interfaces;
using Domain.Features.Projects.Interfaces;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;

namespace HumanResource.UnitTests.Application.Features.MilestoneParticipations
{
    public class CreateMilestoneParticipationHandlerTests
    {
        private readonly Mock<IMilestoneParticipationRepository> _milestoneParticipationRepositoryMock;
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateMilestoneParticipationHandler _handler;

        public CreateMilestoneParticipationHandlerTests()
        {
            _milestoneParticipationRepositoryMock = new Mock<IMilestoneParticipationRepository>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateMilestoneParticipationHandler(
                _milestoneParticipationRepositoryMock.Object,
                _employeeRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenValidData_ShouldCreateParticipationAndReturnResponse()
        {
            // Arrange
            var milestoneId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var command = new CreateMilestoneParticipationCommand
            {
                ProjectMilestoneId = milestoneId,
                EmployeeId = employeeId
            };

            var milestone = new ProjectMilestone(123, "Test Milestone");
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetMilestoneAsync(milestoneId))
                .ReturnsAsync(milestone);

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetByMilestoneAndEmployeeAsync(milestoneId, employeeId))
                .ReturnsAsync((MilestoneParticipation?)null);

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.ProjectMilestoneId.Should().Be(milestoneId);
            result.EmployeeId.Should().Be(employeeId);
            result.IsPaid.Should().BeFalse();
            result.IsActive.Should().BeTrue();

            _milestoneParticipationRepositoryMock.Verify(x => x.AddAsync(It.Is<MilestoneParticipation>(p =>
                p.ProjectMilestoneId == milestoneId &&
                p.EmployeeId == employeeId &&
                p.ProjectMilestone == milestone)), Times.Once);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenMilestoneDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var milestoneId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var command = new CreateMilestoneParticipationCommand
            {
                ProjectMilestoneId = milestoneId,
                EmployeeId = employeeId
            };

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetMilestoneAsync(milestoneId))
                .ReturnsAsync((ProjectMilestone?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Project milestone not found.");
            _milestoneParticipationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MilestoneParticipation>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenEmployeeDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var milestoneId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var command = new CreateMilestoneParticipationCommand
            {
                ProjectMilestoneId = milestoneId,
                EmployeeId = employeeId
            };

            var milestone = new ProjectMilestone(456, "Test Milestone");

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetMilestoneAsync(milestoneId))
                .ReturnsAsync(milestone);

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync((Employee?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be($"Employee {employeeId} does not exist.");
            _milestoneParticipationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MilestoneParticipation>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenParticipationAlreadyExists_ShouldThrowException()
        {
            // Arrange
            var milestoneId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var command = new CreateMilestoneParticipationCommand
            {
                ProjectMilestoneId = milestoneId,
                EmployeeId = employeeId
            };

            var milestone = new ProjectMilestone(456, "Test Milestone");
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);
            
            var existingParticipation = new MilestoneParticipation(milestoneId, employeeId, milestone);

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetMilestoneAsync(milestoneId))
                .ReturnsAsync(milestone);

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetByMilestoneAndEmployeeAsync(milestoneId, employeeId))
                .ReturnsAsync(existingParticipation);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("Participation already exists.");
            _milestoneParticipationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MilestoneParticipation>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenValidData_ShouldCreateParticipationWithCorrectInitialState()
        {
            // Arrange
            var milestoneId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var command = new CreateMilestoneParticipationCommand
            {
                ProjectMilestoneId = milestoneId,
                EmployeeId = employeeId
            };

            var milestone = new ProjectMilestone(123, "Test Milestone");
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetMilestoneAsync(milestoneId))
                .ReturnsAsync(milestone);

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetByMilestoneAndEmployeeAsync(milestoneId, employeeId))
                .ReturnsAsync((MilestoneParticipation?)null);

            MilestoneParticipation? capturedParticipation = null;
            _milestoneParticipationRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<MilestoneParticipation>()))
                .Callback<MilestoneParticipation>(p => capturedParticipation = p);

            // Act
            await _handler.HandleAsync(command);

            // Assert
            capturedParticipation.Should().NotBeNull();
            capturedParticipation!.ProjectMilestoneId.Should().Be(milestoneId);
            capturedParticipation.EmployeeId.Should().Be(employeeId);
            capturedParticipation.ProjectMilestone.Should().Be(milestone);
            capturedParticipation.IsPaid.Should().BeFalse();
            capturedParticipation.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task HandleAsync_WhenValidData_ShouldCallRepositoryMethodsInCorrectOrder()
        {
            // Arrange
            var milestoneId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            var command = new CreateMilestoneParticipationCommand
            {
                ProjectMilestoneId = milestoneId,
                EmployeeId = employeeId
            };

            var milestone = new ProjectMilestone(123, "Test Milestone");
            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetMilestoneAsync(milestoneId))
                .ReturnsAsync(milestone);

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetByMilestoneAndEmployeeAsync(milestoneId, employeeId))
                .ReturnsAsync((MilestoneParticipation?)null);

            // Act
            await _handler.HandleAsync(command);

            // Assert
            var sequence = new MockSequence();
            _milestoneParticipationRepositoryMock.InSequence(sequence)
                .Setup(x => x.GetMilestoneAsync(milestoneId))
                .ReturnsAsync(milestone);
            _employeeRepositoryMock.InSequence(sequence)
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);
            _milestoneParticipationRepositoryMock.InSequence(sequence)
                .Setup(x => x.GetByMilestoneAndEmployeeAsync(milestoneId, employeeId))
                .ReturnsAsync((MilestoneParticipation?)null);
            _milestoneParticipationRepositoryMock.InSequence(sequence)
                .Setup(x => x.AddAsync(It.IsAny<MilestoneParticipation>()));
            _unitOfWorkMock.InSequence(sequence)
                .Setup(x => x.SaveChangesAsync(CancellationToken.None));

            // Verify the sequence was followed
            _milestoneParticipationRepositoryMock.Verify(x => x.GetMilestoneAsync(milestoneId), Times.Once);
            _employeeRepositoryMock.Verify(x => x.GetByIdAsync(employeeId), Times.Once);
            _milestoneParticipationRepositoryMock.Verify(x => x.GetByMilestoneAndEmployeeAsync(milestoneId, employeeId), Times.Once);
            _milestoneParticipationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MilestoneParticipation>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
    }
}
