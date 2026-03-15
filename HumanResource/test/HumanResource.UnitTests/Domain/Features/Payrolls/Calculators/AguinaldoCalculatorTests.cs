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
    public class AguinaldoCalculatorTests
    {
        private const decimal BaseSalaryAmount = 5000m;
        private readonly decimal _expectedMonthlyAccrual = Math.Round(BaseSalaryAmount * (1m / 12m), 2);

        [Fact]
        public void Calculate_WithActiveRule_ShouldAccrueMonthlyAmount()
        {
            // Arrange
            var calculator = new AguinaldoCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithActiveRule(accruedAmount: 0m);

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var accrualComponent = payroll.Components.Last(c => c.Category == PayrollComponentCategory.Accrual);
            accrualComponent.Type.Should().Be(PayrollComponentType.Aguinaldo);
            accrualComponent!.Amount.Should().BeApproximately(_expectedMonthlyAccrual, 0.01m);

            context.AguinaldoBalance.AccruedAmount.Should().BeApproximately(_expectedMonthlyAccrual, 0.01m);
            context.AguinaldoBalance.PaidAmount.Should().Be(0m);
        }

        [Fact]
        public void Calculate_WithPayMonth_ShouldPayAccruedAmount()
        {
            // Arrange
            var calculator = new AguinaldoCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithPayMonth(accruedAmount: 0m);

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(3);
            var accrualComponent = payroll.Components.FirstOrDefault(c => c.Category == PayrollComponentCategory.Accrual);
            var earningComponent = payroll.Components.Last(c => c.Category == PayrollComponentCategory.Earning);
            
            accrualComponent!.Amount.Should().BeApproximately(_expectedMonthlyAccrual, 0.01m);
            earningComponent.Type.Should().Be(PayrollComponentType.Aguinaldo);
            earningComponent.Amount.Should().BeApproximately(_expectedMonthlyAccrual, 0.01m);

            payroll.AguinaldoPayments.Should().HaveCount(1);
            var payment = payroll.AguinaldoPayments.First();
            payment.Amount.Should().BeApproximately(_expectedMonthlyAccrual, 0.01m);

            context.AguinaldoBalance.AccruedAmount.Should().Be(0m);
            context.AguinaldoBalance.PaidAmount.Should().BeApproximately(_expectedMonthlyAccrual, 0.01m);
        }

        [Fact]
        public void Calculate_WithInactiveRule_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new AguinaldoCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithInactiveRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1); 
            payroll.AguinaldoPayments.Should().BeEmpty();

            context.AguinaldoBalance.AccruedAmount.Should().Be(0m);
            context.AguinaldoBalance.PaidAmount.Should().Be(0m);
        }

        [Fact]
        public void Calculate_WithNoRule_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new AguinaldoCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithNoRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.AguinaldoPayments.Should().BeEmpty();

            context.AguinaldoBalance.AccruedAmount.Should().Be(0m);
            context.AguinaldoBalance.PaidAmount.Should().Be(0m);
        }

        [Fact]
        public void Calculate_WithoutBaseSalaryRule_ShouldThrowException()
        {
            // Arrange
            var calculator = new AguinaldoCalculator();
            var payroll = CreateEmptyPayroll();
            var context = CreateContextWithoutBaseSalaryRule();

            // Act
            Action act = () => calculator.Calculate(payroll, context);

            // Assert
            act.Should().Throw<Exception>().WithMessage("*No active base salary rule found for employee role*");
        }

        [Fact]
        public void Calculate_WithExistingBalance_ShouldAccrueOnTop()
        {
            // Arrange
            var calculator = new AguinaldoCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithActiveRule(accruedAmount: 1000m);

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var accrualComponent = payroll.Components.Last(c => c.Category == PayrollComponentCategory.Accrual);
            accrualComponent!.Amount.Should().BeApproximately(_expectedMonthlyAccrual, 0.01m);

            context.AguinaldoBalance.AccruedAmount.Should().BeApproximately(1000m + _expectedMonthlyAccrual, 0.01m);
            context.AguinaldoBalance.PaidAmount.Should().Be(0m);
        }

        [Fact]
        public void Calculate_WithPayMonthAndExistingBalance_ShouldPayTotal()
        {
            // Arrange
            var calculator = new AguinaldoCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithPayMonth(accruedAmount: 1000m);
            var totalExpected = 1000m + _expectedMonthlyAccrual;

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(3);
            var accrualComponent = payroll.Components.FirstOrDefault(c => c.Category == PayrollComponentCategory.Accrual);
            var earningComponent = payroll.Components.Last(c => c.Category == PayrollComponentCategory.Earning);
            
            accrualComponent!.Amount.Should().BeApproximately(_expectedMonthlyAccrual, 0.01m);
            earningComponent.Amount.Should().BeApproximately(totalExpected, 0.01m);

            payroll.AguinaldoPayments.Should().HaveCount(1);
            var payment = payroll.AguinaldoPayments.First();
            payment.Amount.Should().BeApproximately(totalExpected, 0.01m);

            context.AguinaldoBalance.AccruedAmount.Should().Be(0m);
            context.AguinaldoBalance.PaidAmount.Should().BeApproximately(totalExpected, 0.01m);
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
                BaseSalaryAmount,
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

        private static EmployeeAguinaldoBalance CreateEmployeeAguinaldoBalance(decimal accruedAmount)
        {
            var employee = new Employee(
                "Test Employee",
                "test@test.com",
                EmployeeRole.Employee,
                "hash",
                1);
            
            if (accruedAmount > 0)
                employee.AccrueAguinaldo(accruedAmount);
            
            return employee.AguinaldoBalance;
        }

        private PayrollCalculationContext CreateContextWithActiveRule(decimal accruedAmount)
        {
            var aguinaldoRule = new AguinaldoRule(1m / 12m, 12);
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, BaseSalaryAmount);
            
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
                aguinaldoRule: aguinaldoRule,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(accruedAmount),
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

        private PayrollCalculationContext CreateContextWithPayMonth(decimal accruedAmount)
        {
            var aguinaldoRule = new AguinaldoRule(1m / 12m, 12);
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, BaseSalaryAmount);
            
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
                aguinaldoRule: aguinaldoRule,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(accruedAmount),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: new DateTime(2024, 12, 1),
                periodEnd: new DateTime(2024, 12, 31)
            );
        }

        private PayrollCalculationContext CreateContextWithInactiveRule()
        {
            var aguinaldoRule = new AguinaldoRule(1m / 12m, 12);
            aguinaldoRule.Deactivate();
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, BaseSalaryAmount);
            
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
                aguinaldoRule: aguinaldoRule,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(0m),
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

        private PayrollCalculationContext CreateContextWithNoRule()
        {
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, BaseSalaryAmount);
            
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
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(0m),
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

        private PayrollCalculationContext CreateContextWithoutBaseSalaryRule()
        {
            var aguinaldoRule = new AguinaldoRule(1m / 12m, 12);
            
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
                aguinaldoRule: aguinaldoRule,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(0m),
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
    }
}
