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

namespace HumanResource.UnitTests.Domain.Features.Payrolls.Calculators
{
    public class DeductionCalculatorTests
    {
        [Fact]
        public void Calculate_WithBasicSalaryDeductions_ShouldAddComponentsAndPayments()
        {
            // Arrange
            var calculator = new DeductionCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithBasicSalaryDeductions();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var deductionComponent = payroll.Components.Last(c => c.Category == PayrollComponentCategory.Deduction);
            deductionComponent.Type.Should().Be(PayrollComponentType.LegalDeduction);
            deductionComponent.Amount.Should().Be(100m);

            payroll.DeductionPayments.Should().HaveCount(1);
            var payment = payroll.DeductionPayments.First();
            payment.Amount.Should().Be(100m);
        }

        [Fact]
        public void Calculate_WithTotalEarningsDeductions_ShouldCalculateOnGrossAmount()
        {
            // Arrange
            var calculator = new DeductionCalculator();
            var payroll = CreatePayrollWithMultipleEarnings();
            var context = CreateContextWithTotalEarningsDeductions();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(3);
            var deductionComponent = payroll.Components.Last(c => c.Category == PayrollComponentCategory.Deduction);
            deductionComponent.Amount.Should().Be(140m);
        }

        [Fact]
        public void Calculate_WithMultipleDeductions_ShouldAddAllActiveRules()
        {
            // Arrange
            var calculator = new DeductionCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithMultipleDeductions();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(3);
            var deductionComponents = payroll.Components.Where(c => c.Category == PayrollComponentCategory.Deduction);
            deductionComponents.Should().HaveCount(2);
            
            var amounts = deductionComponents.Select(c => c.Amount).ToList();
            amounts.Should().Contain(100m);
            amounts.Should().Contain(150m);

            payroll.DeductionPayments.Should().HaveCount(2);
        }

        [Fact]
        public void Calculate_WithNoEarnings_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new DeductionCalculator();
            var payroll = CreateEmptyPayroll();
            var context = CreateContextWithBasicSalaryDeductions();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().BeEmpty();
            payroll.DeductionPayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithInactiveRules_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new DeductionCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithInactiveRules();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.DeductionPayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithNoRules_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new DeductionCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithNoRules();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.DeductionPayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithMinimalAmount_ShouldAddComponent()
        {
            // Arrange
            var calculator = new DeductionCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithZeroAmount();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var deductionComponent = payroll.Components.Last(c => c.Category == PayrollComponentCategory.Deduction);
            deductionComponent.Amount.Should().Be(0.50m);

            payroll.DeductionPayments.Should().HaveCount(1);
            var payment = payroll.DeductionPayments.First();
            payment.Amount.Should().Be(0.50m);
        }

        private static Payroll CreatePayrollWithBaseSalary()
        {
            var payroll = new Payroll(
                Guid.NewGuid(),
                new DateTime(2024, 1, 1),
                new DateTime(2024, 1, 31));
            
            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.BaseSalary,
                PayrollComponentCategory.Earning,
                "Base Salary",
                5000m,
                Guid.NewGuid()));
            
            return payroll;
        }

        private static Payroll CreatePayrollWithMultipleEarnings()
        {
            var payroll = new Payroll(
                Guid.NewGuid(),
                new DateTime(2024, 1, 1),
                new DateTime(2024, 1, 31));
            
            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.BaseSalary,
                PayrollComponentCategory.Earning,
                "Base Salary",
                5000m,
                Guid.NewGuid()));
            
            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.Overtime,
                PayrollComponentCategory.Earning,
                "Overtime",
                2000m,
                Guid.NewGuid()));
            
            return payroll;
        }

        private static Payroll CreateEmptyPayroll()
        {
            return new Payroll(
                Guid.NewGuid(),
                new DateTime(2024, 1, 1),
                new DateTime(2024, 1, 31));
        }

        private static PayrollCalculationContext CreateContextWithBasicSalaryDeductions()
        {
            var rule = new DeductionRule(
                0.02m,
                "Tax",
                DeductionType.BasicSalary);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule> { rule },
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

        private static PayrollCalculationContext CreateContextWithTotalEarningsDeductions()
        {
            var rule = new DeductionRule(
                0.02m,
                "Social Security",
                DeductionType.TotalEarnings);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule> { rule },
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

        private static PayrollCalculationContext CreateContextWithMultipleDeductions()
        {
            var rule1 = new DeductionRule(
                0.02m,
                "Tax",
                DeductionType.BasicSalary);
            var rule2 = new DeductionRule(
                0.03m,
                "Social Security",
                DeductionType.BasicSalary);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule> { rule1, rule2 },
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

        private static PayrollCalculationContext CreateContextWithInactiveRules()
        {
            var rule = new DeductionRule(
                0.02m,
                "Tax",
                DeductionType.BasicSalary);
            rule.Deactivate();
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule> { rule },
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

        private static PayrollCalculationContext CreateContextWithZeroAmount()
        {
            var rule = new DeductionRule(
                0.0001m,
                "Minimal Deduction",
                DeductionType.BasicSalary);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule> { rule },
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
