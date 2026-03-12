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
    public class ProductivityCalculatorTests
    {
        [Fact]
        public void Calculate_WithFixedAmountBonus_ShouldAddComponentAndPayment()
        {
            // Arrange
            var calculator = new ProductivityCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithFixedBonus();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var productivityComponent = payroll.Components.Last(c => c.Type == PayrollComponentType.ProductivityBonus);
            productivityComponent.Category.Should().Be(PayrollComponentCategory.Earning);
            productivityComponent.Amount.Should().Be(500m);
            productivityComponent.Description.Should().Be("Proportional Productivity Bonus");

            payroll.ProductivityPayments.Should().HaveCount(1);
            var payment = payroll.ProductivityPayments.First();
            payment.Amount.Should().Be(500m);
        }

        [Fact]
        public void Calculate_WithPercentageBonus_ShouldCalculateBasedOnGrossEarnings()
        {
            // Arrange
            var calculator = new ProductivityCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithPercentageBonus();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var productivityComponent = payroll.Components.Last(c => c.Type == PayrollComponentType.ProductivityBonus);
            productivityComponent.Amount.Should().Be(500m);
        }

        [Fact]
        public void Calculate_WithPartialMetric_ShouldApplyProportionalFactor()
        {
            // Arrange
            var calculator = new ProductivityCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithPartialMetric();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var productivityComponent = payroll.Components.Last(c => c.Type == PayrollComponentType.ProductivityBonus);
            productivityComponent.Amount.Should().Be(250m);
        }

        [Fact]
        public void Calculate_WithMetricBelowMinimum_ShouldNotAddComponent()
        {
            // Arrange
            var calculator = new ProductivityCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithLowMetric();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.ProductivityPayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithInactiveRule_ShouldNotAddComponent()
        {
            // Arrange
            var calculator = new ProductivityCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithInactiveRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.ProductivityPayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithNoRule_ShouldNotAddComponent()
        {
            // Arrange
            var calculator = new ProductivityCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithNoRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.ProductivityPayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithMaxBonusCap_ShouldApplyCap()
        {
            // Arrange
            var calculator = new ProductivityCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithMaxCap();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var productivityComponent = payroll.Components.Last(c => c.Type == PayrollComponentType.ProductivityBonus);
            productivityComponent.Amount.Should().Be(300m);
        }

        [Fact]
        public void Calculate_WithZeroOrNegativeResult_ShouldNotAddComponent()
        {
            // Arrange
            var calculator = new ProductivityCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithZeroResult();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.ProductivityPayments.Should().BeEmpty();
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

        private static PayrollCalculationContext CreateContextWithFixedBonus()
        {
            var rule = new ProductivityRule(
                80m,  
                100m, 
                500m, 
                BonusType.FixedAmount,
                null);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 100m,
                productivityRule: rule,
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

        private static PayrollCalculationContext CreateContextWithPercentageBonus()
        {
            var rule = new ProductivityRule(
                80m,  
                100m, 
                10m,  
                BonusType.Percentage,
                null);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 100m, 
                productivityRule: rule,
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

        private static PayrollCalculationContext CreateContextWithPartialMetric()
        {
            var rule = new ProductivityRule(
                80m,  
                100m, 
                500m, 
                BonusType.FixedAmount,
                null);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 90m,
                productivityRule: rule,
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

        private static PayrollCalculationContext CreateContextWithLowMetric()
        {
            var rule = new ProductivityRule(
                80m, 
                100m,
                500m, 
                BonusType.FixedAmount,
                null);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 70m,
                productivityRule: rule,
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
            var rule = new ProductivityRule(
                80m,  
                100m, 
                500m, 
                BonusType.FixedAmount,
                null);
            rule.Deactivate();
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 100m,
                productivityRule: rule,
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

        private static PayrollCalculationContext CreateContextWithMaxCap()
        {
            var rule = new ProductivityRule(
                80m,  
                100m, 
                500m, 
                BonusType.FixedAmount,
                300m);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 100m,
                productivityRule: rule,
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

        private static PayrollCalculationContext CreateContextWithZeroResult()
        {
            var rule = new ProductivityRule(
                80m, 
                100m,
                500m, 
                BonusType.FixedAmount,
                null);
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 80m, 
                productivityRule: rule,
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

        private static PayrollCalculationContext CreateContextWithNoRule()
        {
            var employeeId = Guid.NewGuid();
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 100m,
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
