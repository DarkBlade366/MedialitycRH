using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Rules.BaseSalary.Handlers;
using Application.Features.Payrolls.Rules.BaseSalary.Commands;
using Application.Features.Payrolls.Rules.BaseSalary.DTOs;
using Application.Common.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;

namespace HumanResource.UnitTests.Application.Features.Payrolls.Rules
{
    public class CreateBaseSalaryRuleHandlerTests
    {
        private readonly Mock<IBaseSalaryRuleRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateBaseSalaryRuleHandler _handler;

        public CreateBaseSalaryRuleHandlerTests()
        {
            _repositoryMock = new Mock<IBaseSalaryRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateBaseSalaryRuleHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleAsync_WhenValidData_ShouldCreateRuleSuccessfully()
        {
            // Arrange
            var command = new CreateBaseSalaryRuleCommand
            {
                Role = "Employee",
                Amount = 5000m
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<BaseSalaryRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Role.Should().Be("Employee");
            result.Amount.Should().Be(5000m);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<BaseSalaryRule>(r =>
                r.Role == EmployeeRole.Employee &&
                r.Amount == 5000m &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenActiveRuleExistsForRole_ShouldThrowException()
        {
            // Arrange
            var command = new CreateBaseSalaryRuleCommand
            {
                Role = "Employee",
                Amount = 5000m
            };

            var existingActiveRule = new BaseSalaryRule(EmployeeRole.Employee, 4500m);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<BaseSalaryRule> { existingActiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("There is already an active base salary rule for this role.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<BaseSalaryRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenIdenticalInactiveRuleExists_ShouldThrowException()
        {
            // Arrange
            var command = new CreateBaseSalaryRuleCommand
            {
                Role = "Employee",
                Amount = 5000m
            };

            var existingInactiveRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            existingInactiveRule.Deactivate();

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<BaseSalaryRule> { existingInactiveRule });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.HandleAsync(command));

            exception.Message.Should().Be("A base salary rule for role Employee  with amount $5000 is already disabled; enable it.");

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<BaseSalaryRule>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenDifferentRoleHasActiveRule_ShouldCreateNewRuleSuccessfully()
        {
            // Arrange
            var command = new CreateBaseSalaryRuleCommand
            {
                Role = "ProjectManager",
                Amount = 7000m
            };

            var existingDifferentRoleRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<BaseSalaryRule> { existingDifferentRoleRule });

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Role.Should().Be("ProjectManager");
            result.Amount.Should().Be(7000m);

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<BaseSalaryRule>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("Employee")]
        [InlineData("ProjectManager")]
        [InlineData("Administrator")]
        public async Task HandleAsync_WithDifferentRoles_ShouldCreateRuleSuccessfully(string role)
        {
            // Arrange
            var command = new CreateBaseSalaryRuleCommand
            {
                Role = role,
                Amount = 5000m
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<BaseSalaryRule>());

            // Act
            var result = await _handler.HandleAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Role.Should().Be(role);
            result.Amount.Should().Be(5000m);
            result.IsActive.Should().BeTrue();

            _repositoryMock.Verify(x => x.AddAsync(It.Is<BaseSalaryRule>(r =>
                Enum.Parse(typeof(EmployeeRole), role).ToString() == r.Role.ToString() &&
                r.Amount == 5000m &&
                r.IsActive)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenRoleIsInvalid_ShouldThrowException()
        {
            // Arrange
            var command = new CreateBaseSalaryRuleCommand 
            { 
                Role = "InvalidRole", 
                Amount = 5000m 
            };

            // Act & Assert
            Func<Task> act = async () => await _handler.HandleAsync(command);
            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
