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
using Domain.Features.TimeEntries.Aggregates;
using Xunit;
using FluentAssertions;

namespace Domain.Features.Payrolls.Calculators
{
    public class OvertimeCalculatorTests
    {
        [Fact]
        public void Calculate_WithOvertimeHours_ShouldAddComponentAndPayment()
        {
            // Arrange
            var calculator = new OvertimeCalculator();
            var payroll = CreateValidPayroll();
            var context = CreateContextWithOvertime();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            var component = payroll.Components.First();
            component.Type.Should().Be(PayrollComponentType.Overtime);
            component.Category.Should().Be(PayrollComponentCategory.Earning);
            component.Amount.Should().Be(300m);
            component.Description.Should().Contain("Overtime 10 hours");

            payroll.OvertimePayments.Should().HaveCount(1);
            var payment = payroll.OvertimePayments.First();
            payment.Amount.Should().Be(300m);
        }

        [Fact]
        public void Calculate_WithNoOvertimeHours_ShouldNotAddComponent()
        {
            // Arrange
            var calculator = new OvertimeCalculator();
            var payroll = CreateValidPayroll();
            var context = CreateContextWithoutOvertime();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().BeEmpty();
            payroll.OvertimePayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithNoActiveRule_ShouldNotAddComponent()
        {
            // Arrange
            var calculator = new OvertimeCalculator();
            var payroll = CreateValidPayroll();
            var context = CreateContextWithInactiveRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().BeEmpty();
            payroll.OvertimePayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithNoRules_ShouldNotAddComponent()
        {
            // Arrange
            var calculator = new OvertimeCalculator();
            var payroll = CreateValidPayroll();
            var context = CreateContextWithNoRules();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().BeEmpty();
            payroll.OvertimePayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithMultipleRules_ShouldUseFirstActiveRule()
        {
            // Arrange
            var calculator = new OvertimeCalculator();
            var payroll = CreateValidPayroll();
            var context = CreateContextWithMultipleRules();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            var component = payroll.Components.First();
            component.Amount.Should().Be(400m);
        }

        [Fact]
        public void Calculate_WithZeroWorkedHours_ShouldNotAddComponent()
        {
            // Arrange
            var calculator = new OvertimeCalculator();
            var payroll = CreateValidPayroll();
            var context = CreateContextWithZeroHours();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().BeEmpty();
            payroll.OvertimePayments.Should().BeEmpty();
        }

        private static Payroll CreateValidPayroll()
        {
            return new Payroll(
                Guid.NewGuid(),
                new DateTime(2024, 1, 1),
                new DateTime(2024, 1, 31));
        }

        private static PayrollCalculationContext CreateContextWithOvertime()
        {
            var rule = new OvertimeRule(160, 1.5m, 20m);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 170m,
                overtimeRules: new List<OvertimeRule> { rule },
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
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithoutOvertime()
        {
            var rule = new OvertimeRule(160, 1.5m, 20m);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule> { rule },
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
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithInactiveRule()
        {
            var rule = new OvertimeRule(160, 1.5m, 20m);
            rule.Deactivate();
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 170m,
                overtimeRules: new List<OvertimeRule> { rule },
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
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithMultipleRules()
        {
            var rule1 = new OvertimeRule(160, 1.5m, 20m);
            rule1.Deactivate();
            var rule2 = new OvertimeRule(160, 2.0m, 20m);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 170m,
                overtimeRules: new List<OvertimeRule> { rule1, rule2 },
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
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithZeroHours()
        {
            var rule = new OvertimeRule(160, 1.5m, 20m);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 0m,
                overtimeRules: new List<OvertimeRule> { rule },
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
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithNoRules()
        {
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 170m,
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
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
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
