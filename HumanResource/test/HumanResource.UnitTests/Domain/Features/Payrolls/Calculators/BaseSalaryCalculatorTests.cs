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
    public class BaseSalaryCalculatorTests
    {
        [Fact]
        public void Calculate_WithActiveRule_ShouldAddComponent()
        {
            // Arrange
            var calculator = new BaseSalaryCalculator();
            var payroll = CreateValidPayroll();
            var context = CreateContextWithActiveRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            var component = payroll.Components.First();
            component.Type.Should().Be(PayrollComponentType.BaseSalary);
            component.Category.Should().Be(PayrollComponentCategory.Earning);
            component.Amount.Should().Be(5000m);
            component.Description.Should().Contain("Base Salary - Employee");
        }

        [Fact]
        public void Calculate_WithNoActiveRule_ShouldNotAddComponent()
        {
            // Arrange
            var calculator = new BaseSalaryCalculator();
            var payroll = CreateValidPayroll();
            var context = CreateContextWithInactiveRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithNoRules_ShouldNotAddComponent()
        {
            // Arrange
            var calculator = new BaseSalaryCalculator();
            var payroll = CreateValidPayroll();
            var context = CreateContextWithNoRules();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithMultipleRules_ShouldUseFirstActiveRuleForRole()
        {
            // Arrange
            var calculator = new BaseSalaryCalculator();
            var payroll = CreateValidPayroll();
            var context = CreateContextWithMultipleRules();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            var component = payroll.Components.First();
            component.Amount.Should().Be(6000m);
        }

        [Fact]
        public void Calculate_WithDifferentRole_ShouldUseCorrectRule()
        {
            // Arrange
            var calculator = new BaseSalaryCalculator();
            var payroll = CreateValidPayroll();
            var context = CreateContextWithDifferentRole();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            var component = payroll.Components.First();
            component.Amount.Should().Be(7000m);
        }

        private static Payroll CreateValidPayroll()
        {
            return new Payroll(
                Guid.NewGuid(),
                new DateTime(2024, 1, 1),
                new DateTime(2024, 1, 31));
        }

        private static PayrollCalculationContext CreateContextWithActiveRule()
        {
            var rule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { rule },
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
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithInactiveRule()
        {
            var rule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            rule.Deactivate();
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { rule },
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
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithMultipleRules()
        {
            var rule1 = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            rule1.Deactivate();
            var rule2 = new BaseSalaryRule(EmployeeRole.Employee, 6000m);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { rule1, rule2 },
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
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithDifferentRole()
        {
            var employeeRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var managerRule = new BaseSalaryRule(EmployeeRole.ProjectManager, 7000m);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { employeeRule, managerRule },
                employeeRole: EmployeeRole.ProjectManager,
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
