using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Payroll.Handlers;
using Application.Features.Payrolls.Payroll.Commands;
using Application.Features.Payrolls.Payroll.DTOs;
using Application.Common.Interfaces;
using Application.Services;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Payrolls.Services.Calculators;
using Domain.Features.Payrolls.Services.Engines;
using Domain.Features.Payrolls.Services.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Domain.Features.TimeEntries.Interfaces;
using Domain.Features.Projects.Interfaces;
using Domain.Features.Projects.Enums;
using Domain.Features.Projects.Aggregates;

// Alias para evitar conflicto de namespaces
using PayrollAggregate = global::Domain.Features.Payrolls.Aggregates.Payroll;

namespace HumanResource.UnitTests.Application.Features.Payrolls.Payroll
{
    public class CreatePayrollHandlerTests
    {
        private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
        private readonly Mock<IPayrollRepository> _payrollRepositoryMock;
        private readonly Mock<ITimeEntryRepository> _timeEntryRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBaseSalaryRuleRepository> _baseSalaryRuleRepositoryMock;
        private readonly Mock<IOvertimeRuleRepository> _overtimeRuleRepositoryMock;
        private readonly Mock<IDeductionRuleRepository> _deductionRuleRepositoryMock;
        private readonly Mock<IProductivityRuleRepository> _productivityRuleRepositoryMock;
        private readonly Mock<IVacationRuleRepository> _vacationRuleRepositoryMock;
        private readonly Mock<IAguinaldoRuleRepository> _aguinaldoRuleRepositoryMock;
        private readonly Mock<IMilestoneRuleRepository> _milestoneRuleRepositoryMock;
        private readonly Mock<IProjectMilestoneRepository> _projectMilestoneRepositoryMock;
        private readonly Mock<IMilestoneParticipationRepository> _milestoneParticipationRepositoryMock;
        private readonly Mock<IProjectRuleRepository> _projectRuleRepositoryMock;
        private readonly Mock<IProjectRepository> _projectRepositoryMock;
        private readonly Mock<IActivityProductivityWeightRepository> _activityWeightRepositoryMock;
        private readonly ProductivityService _productivityService;
        private readonly PayrollEngine _payrollEngine;
        private readonly CreatePayrollHandler _handler;

        public CreatePayrollHandlerTests()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _payrollRepositoryMock = new Mock<IPayrollRepository>();
            _timeEntryRepositoryMock = new Mock<ITimeEntryRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _baseSalaryRuleRepositoryMock = new Mock<IBaseSalaryRuleRepository>();
            _overtimeRuleRepositoryMock = new Mock<IOvertimeRuleRepository>();
            _deductionRuleRepositoryMock = new Mock<IDeductionRuleRepository>();
            _productivityRuleRepositoryMock = new Mock<IProductivityRuleRepository>();
            _vacationRuleRepositoryMock = new Mock<IVacationRuleRepository>();
            _aguinaldoRuleRepositoryMock = new Mock<IAguinaldoRuleRepository>();
            _milestoneRuleRepositoryMock = new Mock<IMilestoneRuleRepository>();
            _projectMilestoneRepositoryMock = new Mock<IProjectMilestoneRepository>();
            _milestoneParticipationRepositoryMock = new Mock<IMilestoneParticipationRepository>();
            _projectRuleRepositoryMock = new Mock<IProjectRuleRepository>();
            _projectRepositoryMock = new Mock<IProjectRepository>();
            _activityWeightRepositoryMock = new Mock<IActivityProductivityWeightRepository>();

            _activityWeightRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ActivityProductivityWeight>());

            _timeEntryRepositoryMock
                .Setup(x => x.GetHoursByActivityAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new Dictionary<int, decimal>());

            _projectMilestoneRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProjectMilestone>());

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetByEmployeeIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<MilestoneParticipation>());

            _projectRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProjectRule>());

            _projectRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<Project>());

            _productivityService = new ProductivityService(
                _timeEntryRepositoryMock.Object,
                _activityWeightRepositoryMock.Object);

            var earningCalculators = new List<IEarningCalculator>
            {
                new BaseSalaryCalculator(),
                new OvertimeCalculator(),
                new ProductivityCalculator(),
                new MilestoneCalculator(),
                new AguinaldoCalculator(),
                new VacationCalculator(),
                new ProjectCalculator()
            };
            var deductionCalculators = new List<IDeductionCalculator> { new DeductionCalculator() };
            _payrollEngine = new PayrollEngine(earningCalculators, deductionCalculators);

            _handler = new CreatePayrollHandler(
                _employeeRepositoryMock.Object,
                _payrollRepositoryMock.Object,
                _timeEntryRepositoryMock.Object,
                _payrollEngine,
                _unitOfWorkMock.Object,
                _baseSalaryRuleRepositoryMock.Object,
                _overtimeRuleRepositoryMock.Object,
                _deductionRuleRepositoryMock.Object,
                _productivityRuleRepositoryMock.Object,
                _vacationRuleRepositoryMock.Object,
                _aguinaldoRuleRepositoryMock.Object,
                _milestoneRuleRepositoryMock.Object,
                _projectMilestoneRepositoryMock.Object,
                _milestoneParticipationRepositoryMock.Object,
                _productivityService,
                _projectRuleRepositoryMock.Object,
                _projectRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidData_ShouldCreatePayrollAndReturnResponse()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now.AddDays(-1);
            var command = new CreatePayrollCommand
            {
                employeeId = employeeId,
                periodStart = periodStart,
                periodEnd = periodEnd
            };

            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);

            employee.AccrueVacationDays(10m);
            employee.AccrueAguinaldo(1000m);

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            _payrollRepositoryMock
                .Setup(x => x.ExistsOverlappingPayroll(employeeId, periodStart, periodEnd))
                .ReturnsAsync(false);

            _timeEntryRepositoryMock
                .Setup(x => x.HasPendingEntries(employeeId, periodStart, periodEnd))
                .ReturnsAsync(false);

            _timeEntryRepositoryMock
                .Setup(x => x.GetWorkedHours(employeeId, periodStart, periodEnd))
                .ReturnsAsync(160m);

            SetupMockRepositories();

            _payrollRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<PayrollAggregate>()))
                .Callback<PayrollAggregate>(p => { });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EmployeeId.Should().Be(employeeId);
            result.PeriodStart.Should().Be(periodStart);
            result.PeriodEnd.Should().Be(periodEnd);
            result.Status.Should().Be("Calculated");
            result.GrossAmount.Should().BeGreaterThan(0);
            result.NetAmount.Should().BeGreaterThan(0);

            _payrollRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PayrollAggregate>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenInvalidPeriod_ShouldThrowException()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now;
            var periodEnd = DateTime.Now.AddDays(-1);
            var command = new CreatePayrollCommand
            {
                employeeId = employeeId,
                periodStart = periodStart,
                periodEnd = periodEnd
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("Invalid payroll period.");
            _payrollRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PayrollAggregate>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenOverlappingPayrollExists_ShouldThrowException()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now.AddDays(-1);
            var command = new CreatePayrollCommand
            {
                employeeId = employeeId,
                periodStart = periodStart,
                periodEnd = periodEnd
            };

            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            _payrollRepositoryMock
                .Setup(x => x.ExistsOverlappingPayroll(employeeId, periodStart, periodEnd))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("Payroll period overlaps with existing payroll.");
            _payrollRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PayrollAggregate>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenPendingTimeEntriesExist_ShouldThrowException()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now.AddDays(-1);
            var command = new CreatePayrollCommand
            {
                employeeId = employeeId,
                periodStart = periodStart,
                periodEnd = periodEnd
            };

            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            _payrollRepositoryMock
                .Setup(x => x.ExistsOverlappingPayroll(employeeId, periodStart, periodEnd))
                .ReturnsAsync(false);

            _timeEntryRepositoryMock
                .Setup(x => x.HasPendingEntries(employeeId, periodStart, periodEnd))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("There are time entries pending approval for this period.");
            _payrollRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PayrollAggregate>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenEmployeeDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now.AddDays(-1);
            var command = new CreatePayrollCommand
            {
                employeeId = employeeId,
                periodStart = periodStart,
                periodEnd = periodEnd
            };

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync((Employee?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("Employee not found.");
            _payrollRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PayrollAggregate>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenEmployeeHasNoActiveRules_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now.AddDays(-1);
            var command = new CreatePayrollCommand
            {
                employeeId = employeeId,
                periodStart = periodStart,
                periodEnd = periodEnd
            };

            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            _payrollRepositoryMock
                .Setup(x => x.ExistsOverlappingPayroll(employeeId, periodStart, periodEnd))
                .ReturnsAsync(false);

            _timeEntryRepositoryMock
                .Setup(x => x.HasPendingEntries(employeeId, periodStart, periodEnd))
                .ReturnsAsync(false);

            _timeEntryRepositoryMock
                .Setup(x => x.GetWorkedHours(employeeId, periodStart, periodEnd))
                .ReturnsAsync(160m);

            _baseSalaryRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<BaseSalaryRule>());

            _overtimeRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<OvertimeRule>());

            _deductionRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<DeductionRule>());

            _productivityRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductivityRule>());

            _vacationRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule>());

            _aguinaldoRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<AguinaldoRule>());

            _milestoneRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<MilestoneRule>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Be("Payroll must contain at least one earning.");
            _payrollRepositoryMock.Verify(x => x.AddAsync(It.IsAny<PayrollAggregate>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenValidData_ShouldApplyAllActiveRules()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now.AddDays(-1);
            var command = new CreatePayrollCommand
            {
                employeeId = employeeId,
                periodStart = periodStart,
                periodEnd = periodEnd
            };

            var employee = new Employee(
                "John Doe",
                "john@example.com",
                EmployeeRole.Employee,
                "hashedPassword",
                123);

            employee.AccrueVacationDays(10m);
            employee.AccrueAguinaldo(1000m);

            _employeeRepositoryMock
                .Setup(x => x.GetByIdAsync(employeeId))
                .ReturnsAsync(employee);

            _payrollRepositoryMock
                .Setup(x => x.ExistsOverlappingPayroll(employeeId, periodStart, periodEnd))
                .ReturnsAsync(false);

            _timeEntryRepositoryMock
                .Setup(x => x.HasPendingEntries(employeeId, periodStart, periodEnd))
                .ReturnsAsync(false);

            _timeEntryRepositoryMock
                .Setup(x => x.GetWorkedHours(employeeId, periodStart, periodEnd))
                .ReturnsAsync(160m);

            SetupMockRepositories();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Calculated");
            result.GrossAmount.Should().BeGreaterThan(0);
            result.Components.Should().NotBeEmpty();

            _baseSalaryRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _overtimeRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _deductionRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _productivityRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _vacationRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _aguinaldoRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _milestoneRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _projectMilestoneRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _milestoneParticipationRepositoryMock.Verify(x => x.GetByEmployeeIdAsync(employeeId), Times.Once);
            _projectRuleRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            _projectRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        private void SetupMockRepositories()
        {
            // Base Salary Rules
            var baseSalaryRules = new List<BaseSalaryRule>
            {
                new BaseSalaryRule(EmployeeRole.Employee, 3000m)
            };

            _baseSalaryRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(baseSalaryRules);

            // Overtime Rules
            var overtimeRules = new List<OvertimeRule>
            {
                new OvertimeRule(160, 1.5m, 25m)
            };

            _overtimeRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(overtimeRules);

            // Deduction Rules
            var deductionRules = new List<DeductionRule>
            {
                new DeductionRule(0.08m, "Social Security", DeductionType.BasicSalary)
            };

            _deductionRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(deductionRules);

            // Productivity Rules
            var productivityRule = new ProductivityRule(50m, 100m, 50m, BonusType.FixedAmount);

            _productivityRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductivityRule> { productivityRule });

            // Vacation Rules
            var vacationRule = new VacationRule(1.5m);

            _vacationRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<VacationRule> { vacationRule });

            // Aguinaldo Rules
            var aguinaldoRule = new AguinaldoRule(0.5m, 12);

            _aguinaldoRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<AguinaldoRule> { aguinaldoRule });

            // Milestone Rules
            var milestoneRules = new List<MilestoneRule>
            {
                new MilestoneRule(123, "Project Completion", 1000m)
            };

            _milestoneRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(milestoneRules);

            // Project Milestones
            var projectMilestones = new List<ProjectMilestone>
            {
                new ProjectMilestone(123, "Test Project")
            };

            _projectMilestoneRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(projectMilestones);

            // Milestone Participations
            var participations = new List<MilestoneParticipation>();

            _milestoneParticipationRepositoryMock
                .Setup(x => x.GetByEmployeeIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(participations);

            // Project Rules
            var projectRules = new List<ProjectRule>
            {
                new ProjectRule(123, 500m)
            };

            _projectRuleRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(projectRules);

            // Projects
            var projects = new List<Project>
            {
                new Project(123, "Test Project")
            };

            _projectRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(projects);
        }
    }
}