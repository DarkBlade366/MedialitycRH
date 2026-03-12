using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Payrolls.Rules;
using Application.Common.Interfaces;

namespace Application.Services
{
    public class VacationAccrualServiceTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepoMock;
        private readonly Mock<IVacationRuleRepository> _ruleRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VacationAccrualService _service;

        public VacationAccrualServiceTests()
        {
            _employeeRepoMock = new Mock<IEmployeeRepository>();
            _ruleRepoMock = new Mock<IVacationRuleRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service = new VacationAccrualService(
                _employeeRepoMock.Object,
                _ruleRepoMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task AccrueVacationsAsync_WhenNoActiveEmployees_ShouldNotAccrue()
        {
            // Arrange
            _employeeRepoMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(new List<Employee>());

            _ruleRepoMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule>());

            // Act
            await _service.AccrueVacationsAsync();

            // Assert
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AccrueVacationsAsync_WhenNoActiveRules_ShouldNotAccrue()
        {
            // Arrange
            var employees = new List<Employee>
            {
                CreateTestEmployee(Guid.NewGuid(), "John", "Doe")
            };

            _employeeRepoMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(employees);

            _ruleRepoMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule>());

            // Act
            await _service.AccrueVacationsAsync();

            // Assert
            employees[0].VacationBalance.AccruedDays.Should().Be(0m);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AccrueVacationsAsync_WithEmployeesAndActiveRule_ShouldAccrueCorrectly()
        {
            // Arrange
            var employees = new List<Employee>
            {
                CreateTestEmployee(Guid.NewGuid(), "John", "Doe"),
                CreateTestEmployee(Guid.NewGuid(), "Jane", "Smith")
            };

            var rule = new VacationRule(1.5m);

            _employeeRepoMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(employees);

            _ruleRepoMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule> { rule });

            // Act
            await _service.AccrueVacationsAsync();

            // Assert
            employees[0].VacationBalance.AccruedDays.Should().Be(1.5m);
            employees[1].VacationBalance.AccruedDays.Should().Be(1.5m);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AccrueVacationsAsync_WithMultipleRules_ShouldUseActiveRule()
        {
            // Arrange
            var employees = new List<Employee>
            {
                CreateTestEmployee(Guid.NewGuid(), "John", "Doe")
            };

            var rules = new List<VacationRule>
            {
                new VacationRule(2.0m),
                new VacationRule(1.5m)
            };

            // Deactivate the first rule
            rules[0].Deactivate();

            _employeeRepoMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(employees);

            _ruleRepoMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(rules);

            // Act
            await _service.AccrueVacationsAsync();

            // Assert
            employees[0].VacationBalance.AccruedDays.Should().Be(1.5m); // Should use active rule
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData(0.5)]
        [InlineData(1.0)]
        [InlineData(1.5)]
        [InlineData(2.5)]
        public async Task AccrueVacationsAsync_WithDifferentAccrualRates_ShouldApplyCorrectly(decimal accrualRate)
        {
            // Arrange
            var employees = new List<Employee>
            {
                CreateTestEmployee(Guid.NewGuid(), "John", "Doe")
            };

            var rule = new VacationRule(accrualRate);

            _employeeRepoMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(employees);

            _ruleRepoMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule> { rule });

            // Act
            await _service.AccrueVacationsAsync();

            // Assert
            employees[0].VacationBalance.AccruedDays.Should().Be(accrualRate);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AccrueVacationsAsync_WhenEmployeeAlreadyAccruedThisMonth_ShouldSkip()
        {
            // Arrange
            var employee = CreateTestEmployee(Guid.NewGuid(), "John", "Doe");
            employee.AccrueVacationDays(1.5m); // ya acumuló este mes
            var employees = new List<Employee> { employee };
            var rule = new VacationRule(1.5m);

            _employeeRepoMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(employees);

            _ruleRepoMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule> { rule });

            // Act
            await _service.AccrueVacationsAsync();

            // Assert
            employee.VacationBalance.AccruedDays.Should().Be(1.5m); // sin cambios
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AccrueVacationsAsync_WhenRuleIsInactive_ShouldNotAccrue()
        {
            // Arrange
            var employees = new List<Employee>
            {
                CreateTestEmployee(Guid.NewGuid(), "John", "Doe")
            };

            var rule = new VacationRule(1.5m);
            rule.Deactivate();

            _employeeRepoMock
                .Setup(x => x.GetAllActiveAsync())
                .ReturnsAsync(employees);

            _ruleRepoMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule> { rule });

            // Act
            await _service.AccrueVacationsAsync();

            // Assert
            employees[0].VacationBalance.AccruedDays.Should().Be(0m);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private static Employee CreateTestEmployee(Guid id, string firstName, string lastName)
        {
            return new Employee(
                $"{firstName} {lastName}",
                "test@example.com",
                Domain.Features.Employees.Enums.EmployeeRole.Employee,
                "hashedpassword",
                123);
        }
    }
}
