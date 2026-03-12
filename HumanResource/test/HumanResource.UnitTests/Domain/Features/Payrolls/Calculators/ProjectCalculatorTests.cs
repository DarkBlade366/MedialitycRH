using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Entities;
using Domain.Features.Employees.Enums;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Services.Calculators;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Projects.Aggregates;
using Domain.Features.Projects.Enums;
using Domain.Features.TimeEntries.Aggregates;
using Xunit;
using FluentAssertions;

namespace Domain.Features.Payrolls.Calculators
{
    public class ProjectCalculatorTests
    {
        private static (DateTime Start, DateTime End) GetTestPeriod()
        {
            var now = DateTime.UtcNow;
            return (now.AddDays(-5), now.AddDays(5));
        }

        [Fact]
        public void Calculate_WithCompletedProjectAndEmployeeParticipating_ShouldAddProjectBonusComponentAndPayment()
        {
            // Arrange
            var calculator = new ProjectCalculator();
            var employeeId = Guid.NewGuid();
            var (start, end) = GetTestPeriod();
            var payroll = CreatePayrollWithBaseSalary(employeeId, (start, end));
            var context = CreateContextWithCompletedProject(employeeId, (start, end));

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var bonusComponent = payroll.Components.Last(c => c.Type == PayrollComponentType.ProjectBonus);
            bonusComponent.Amount.Should().Be(9000m);

            payroll.ProjectPayments.Should().HaveCount(1);
            var payment = payroll.ProjectPayments.First();
            payment.Amount.Should().Be(9000m);
        }

        [Fact]
        public void Calculate_WithNoCompletedProjects_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new ProjectCalculator();
            var (start, end) = GetTestPeriod();
            var payroll = CreatePayrollWithBaseSalary(Guid.NewGuid(), (start, end));
            var context = CreateContextWithNoCompletedProjects((start, end));

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.ProjectPayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithInactiveRule_ShouldRegisterZeroPayment()
        {
            // Arrange
            var calculator = new ProjectCalculator();
            var (start, end) = GetTestPeriod();
            var payroll = CreatePayrollWithBaseSalary(Guid.NewGuid(), (start, end));
            var context = CreateContextWithInactiveRule((start, end));

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.ProjectPayments.Should().HaveCount(1);
            var payment = payroll.ProjectPayments.First();
            payment!.Amount.Should().Be(0m);
        }

        [Fact]
        public void Calculate_WithNoRule_ShouldRegisterZeroPayment()
        {
            // Arrange
            var calculator = new ProjectCalculator();
            var (start, end) = GetTestPeriod();
            var payroll = CreatePayrollWithBaseSalary(Guid.NewGuid(), (start, end));
            var context = CreateContextWithNoRule((start, end));

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.ProjectPayments.Should().HaveCount(1);
            var payment = payroll.ProjectPayments.First();
            payment!.Amount.Should().Be(0m);
        }

        [Fact]
        public void Calculate_WithProjectAlreadyPaidInThisPayroll_ShouldNotAddDuplicatePayment()
        {
            // Arrange
            var calculator = new ProjectCalculator();
            var employeeId = Guid.NewGuid();
            var (start, end) = GetTestPeriod();
            var payroll = CreatePayrollWithBaseSalary(employeeId, (start, end));
            var rule = new ProjectRule(123, 9000m);
            payroll.AddProjectPayment(123, 3000m, DateTime.UtcNow);

            var context = CreateContextWithCompletedProject(employeeId, (start, end), rule);

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.ProjectPayments.Should().HaveCount(1); 
            payroll.ProjectPayments.First().Amount.Should().Be(3000m); 
        }

        [Fact]
        public void Calculate_WithNoParticipants_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new ProjectCalculator();
            var (start, end) = GetTestPeriod();
            var payroll = CreatePayrollWithBaseSalary(Guid.NewGuid(), (start, end));
            var context = CreateContextWithNoParticipants((start, end));

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1); 
            payroll.ProjectPayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithMultipleParticipants_ShouldAddProportionalBonus()
        {
            // Arrange
            var calculator = new ProjectCalculator();
            var employeeId = Guid.NewGuid();
            var (start, end) = GetTestPeriod();
            var payroll = CreatePayrollWithBaseSalary(employeeId, (start, end));
            var context = CreateContextWithMultipleParticipants(employeeId, (start, end));

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var bonusComponent = payroll.Components.Last(c => c.Type == PayrollComponentType.ProjectBonus);
            bonusComponent.Amount.Should().Be(4500m);

            payroll.ProjectPayments.Should().HaveCount(1);
            var payment = payroll.ProjectPayments.First();
            payment.Amount.Should().Be(4500m);
        }

        [Fact]
        public void Calculate_WithEmployeeNotParticipating_ShouldNotAddComponentButRegisterPayment()
        {
            // Arrange
            var calculator = new ProjectCalculator();
            var (start, end) = GetTestPeriod();
            var payroll = CreatePayrollWithBaseSalary(Guid.NewGuid(), (start, end));
            var context = CreateContextWithEmployeeNotParticipating((start, end));

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.ProjectPayments.Should().HaveCount(1);
            var payment = payroll.ProjectPayments.First();
            payment!.Amount.Should().Be(9000m);
        }

        private static Payroll CreatePayrollWithBaseSalary(Guid employeeId, (DateTime Start, DateTime End) period)
        {
            var payroll = new Payroll(
                employeeId,
                period.Start,
                period.End);
            
            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.BaseSalary,
                PayrollComponentCategory.Earning,
                "Base Salary",
                5000m,
                Guid.NewGuid()));
            
            return payroll;
        }

        private static PayrollCalculationContext CreateContextWithCompletedProject(Guid employeeId, (DateTime Start, DateTime End) period, ProjectRule? projectRule = null)
        {
            projectRule ??= new ProjectRule(123, 9000m);
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            
            var project = new Project(123, "Test Project");
            project.UpdateStatus(ProjectStatus.Completed);
            
            var timeEntry = new TimeEntry(
                1,
                123,
                employeeId,
                8m,
                DateTime.UtcNow.AddDays(-1));
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { baseSalaryRule },
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: null,
                vacationBalance: null,
                vacationDaysUsed: 0m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule> { projectRule },
                completedProjects: new List<Project> { project },
                timeEntries: new List<TimeEntry> { timeEntry },
                periodStart: period.Start,
                periodEnd: period.End
            );
        }

        private static PayrollCalculationContext CreateContextWithNoCompletedProjects((DateTime Start, DateTime End) period)
        {
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var employeeId = Guid.NewGuid();
            
            var project = new Project(
                123,
                "Test Project",
                ProjectStatus.Active);
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { baseSalaryRule },
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: null,
                vacationBalance: null,
                vacationDaysUsed: 0m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: period.Start,
                periodEnd: period.End
            );
        }

        private static PayrollCalculationContext CreateContextWithInactiveRule((DateTime Start, DateTime End) period)
        {
            var projectRule = new ProjectRule(123, 9000m);
            projectRule.Deactivate();
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var employeeId = Guid.NewGuid();
            
            var project = new Project(123, "Test Project");
            project.UpdateStatus(ProjectStatus.Completed);
            
            var timeEntry = new TimeEntry(
                1,
                123,
                employeeId,
                8m,
                DateTime.UtcNow.AddDays(-1));
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { baseSalaryRule },
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: null,
                vacationBalance: null,
                vacationDaysUsed: 0m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule> { projectRule },
                completedProjects: new List<Project> { project },
                timeEntries: new List<TimeEntry> { timeEntry },
                periodStart: period.Start,
                periodEnd: period.End
            );
        }

        private static PayrollCalculationContext CreateContextWithNoRule((DateTime Start, DateTime End) period)
        {
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var employeeId = Guid.NewGuid();
            
            var project = new Project(123, "Test Project");
            project.UpdateStatus(ProjectStatus.Completed);
            
            var timeEntry = new TimeEntry(
                1,
                123,
                employeeId,
                8m,
                DateTime.UtcNow.AddDays(-1));
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { baseSalaryRule },
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: null,
                vacationBalance: null,
                vacationDaysUsed: 0m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project> { project },
                timeEntries: new List<TimeEntry> { timeEntry },
                periodStart: period.Start,
                periodEnd: period.End
            );
        }

        private static PayrollCalculationContext CreateContextWithNoParticipants((DateTime Start, DateTime End) period)
        {
            var projectRule = new ProjectRule(123, 9000m);
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var employeeId = Guid.NewGuid();
            
            var project = new Project(123, "Test Project");
            project.UpdateStatus(ProjectStatus.Completed);
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { baseSalaryRule },
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: null,
                vacationBalance: null,
                vacationDaysUsed: 0m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule> { projectRule },
                completedProjects: new List<Project> { project },
                timeEntries: new List<TimeEntry>(),
                periodStart: period.Start,
                periodEnd: period.End
            );
        }

        private static PayrollCalculationContext CreateContextWithMultipleParticipants(Guid employeeId, (DateTime Start, DateTime End) period)
        {
            var projectRule = new ProjectRule(123, 9000m);
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var otherEmployeeId = Guid.NewGuid();
            
            var project = new Project(123, "Test Project");
            project.UpdateStatus(ProjectStatus.Completed);
            
            var timeEntry1 = new TimeEntry(
                1,
                123,
                employeeId,
                8m,
                DateTime.UtcNow.AddDays(-1));
            
            var timeEntry2 = new TimeEntry(
                2,
                123,
                otherEmployeeId,
                8m,
                DateTime.UtcNow.AddDays(-1));
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { baseSalaryRule },
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: null,
                vacationBalance: null,
                vacationDaysUsed: 0m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule> { projectRule },
                completedProjects: new List<Project> { project },
                timeEntries: new List<TimeEntry> { timeEntry1, timeEntry2 },
                periodStart: period.Start,
                periodEnd: period.End
            );
        }

        private static PayrollCalculationContext CreateContextWithEmployeeNotParticipating((DateTime Start, DateTime End) period)
        {
            var projectRule = new ProjectRule(123, 9000m);
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var employeeId = Guid.NewGuid();
            var otherEmployeeId = Guid.NewGuid();
            
            var project = new Project(123, "Test Project");
            project.UpdateStatus(ProjectStatus.Completed);
            
            var timeEntry = new TimeEntry(
                1,
                123,
                otherEmployeeId,
                8m,
                DateTime.UtcNow.AddDays(-1));
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { baseSalaryRule },
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: null,
                vacationBalance: null,
                vacationDaysUsed: 0m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule> { projectRule },
                completedProjects: new List<Project> { project },
                timeEntries: new List<TimeEntry> { timeEntry },
                periodStart: period.Start,
                periodEnd: period.End
            );
        }

        private static EmployeeAguinaldoBalance CreateEmployeeAguinaldoBalance(Guid employeeId, decimal accruedAmount = 0m)
        {
            var employee = new Employee(
                "Test Employee",
                "test@test.com",
                EmployeeRole.Employee,
                "hashedPassword123",
                12345);
            
            if (accruedAmount > 0)
                employee.AccrueAguinaldo(accruedAmount);
            
            return employee.AguinaldoBalance;
        }
    }
}
