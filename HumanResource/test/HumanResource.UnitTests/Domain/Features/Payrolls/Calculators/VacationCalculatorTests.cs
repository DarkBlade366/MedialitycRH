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
    public class VacationCalculatorTests
    {
        private static (DateTime Start, DateTime End) GetTestPeriod()
        {
            var now = DateTime.UtcNow;
            return (now.AddDays(-5), now.AddDays(5));
        }
        [Fact]
        public void Calculate_WithActiveRule_ShouldAddVacationPayment()
        {
            // Arrange
            var calculator = new VacationCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithActiveRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var vacationComponent = payroll.Components.Last(c => c.Category == PayrollComponentCategory.Earning);
            vacationComponent!.Type.Should().Be(PayrollComponentType.VacationPay);
            vacationComponent!.Amount.Should().BeApproximately(833.33m, 0.01m);

            payroll.VacationPayments.Should().HaveCount(1);
            var payment = payroll.VacationPayments.First();
            payment!.Amount.Should().BeApproximately(833.33m, 0.01m);
        }

        [Fact]
        public void Calculate_WithNoVacationDays_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new VacationCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithNoVacationDays();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.VacationPayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithInactiveRule_ShouldStillAddComponents()
        {
            // Arrange
            var calculator = new VacationCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithInactiveRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            payroll.VacationPayments.Should().HaveCount(1);
            var payment = payroll.VacationPayments.First();
            payment!.Amount.Should().BeApproximately(833.33m, 0.01m);
        }

        [Fact]
        public void Calculate_WithNoRule_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new VacationCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithNoRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.VacationPayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithNoBaseSalaryRule_ShouldThrowException()
        {
            // Arrange
            var calculator = new VacationCalculator();
            var payroll = CreateEmptyPayroll();
            var context = CreateContextWithActiveRule(includeBaseSalary: false);

            // Act
            Action act = () => calculator.Calculate(payroll, context);

            // Assert
            act.Should().Throw<Exception>().WithMessage("*No active base salary rule found for employee role*");
        }

        [Fact]
        public void Calculate_WithPartialVacationDays_ShouldAddProportionalPayment()
        {
            // Arrange
            var calculator = new VacationCalculator();
            var payroll = CreatePayrollWithBaseSalary();
            var context = CreateContextWithPartialVacationDays();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var vacationComponent = payroll.Components.Last(c => c.Category == PayrollComponentCategory.Earning);
            vacationComponent!.Type.Should().Be(PayrollComponentType.VacationPay);
            vacationComponent!.Amount.Should().BeApproximately(416.67m, 0.01m);

            payroll.VacationPayments.Should().HaveCount(1);
            var payment = payroll.VacationPayments.First();
            payment!.Amount.Should().BeApproximately(416.67m, 0.01m);
        }

        private static Payroll CreatePayrollWithBaseSalary()
        {
            var (start, end) = GetTestPeriod();
            var payroll = new Payroll(
                Guid.NewGuid(),
                start,
                end);
            
            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.BaseSalary,
                PayrollComponentCategory.Earning,
                "Base Salary",
                5000m,
                Guid.NewGuid()));
            
            return payroll;
        }

        private static Payroll CreateEmptyPayroll()
        {
            var (start, end) = GetTestPeriod();
            return new Payroll(
                Guid.NewGuid(),
                start,
                end);
        }

        private static PayrollCalculationContext CreateContextWithActiveRule(bool includeBaseSalary = true)
        {
            var vacationRule = new VacationRule(1.25m);
            var (start, end) = GetTestPeriod();
            var employeeId = Guid.NewGuid();
            var vacationBalance = CreateEmployeeVacationBalance(employeeId, 0m);
            
            return new PayrollCalculationContext(
                baseSalaryRules: includeBaseSalary ? new List<BaseSalaryRule> { new BaseSalaryRule(EmployeeRole.Employee, 5000m) } : new List<BaseSalaryRule>(),
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: vacationRule,
                vacationBalance: vacationBalance,
                vacationDaysUsed: 5m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: start,
                periodEnd: end
            );
        }

        private static PayrollCalculationContext CreateContextWithNoVacationDays()
        {
            var vacationRule = new VacationRule(1.25m);
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var (start, end) = GetTestPeriod();
            var employeeId = Guid.NewGuid();
            var vacationBalance = CreateEmployeeVacationBalance(employeeId, 0m);
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { baseSalaryRule },
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: vacationRule,
                vacationBalance: vacationBalance,
                vacationDaysUsed: 0m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: start,
                periodEnd: end
            );
        }

        private static PayrollCalculationContext CreateContextWithInactiveRule()
        {
            var vacationRule = new VacationRule(1.25m);
            vacationRule.Deactivate();
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var (start, end) = GetTestPeriod();
            var employeeId = Guid.NewGuid();
            var vacationBalance = CreateEmployeeVacationBalance(employeeId, 0m);
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { baseSalaryRule },
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: vacationRule,
                vacationBalance: vacationBalance,
                vacationDaysUsed: 5m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: start,
                periodEnd: end
            );
        }

        private static PayrollCalculationContext CreateContextWithNoRule()
        {
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var (start, end) = GetTestPeriod();
            var employeeId = Guid.NewGuid();
            var vacationBalance = CreateEmployeeVacationBalance(employeeId, 0m);
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { baseSalaryRule },
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: null,
                vacationBalance: vacationBalance,
                vacationDaysUsed: 5m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: start,
                periodEnd: end
            );
        }

        private static PayrollCalculationContext CreateContextWithPartialVacationDays()
        {
            var vacationRule = new VacationRule(1.25m);
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var (start, end) = GetTestPeriod();
            var employeeId = Guid.NewGuid();
            var vacationBalance = CreateEmployeeVacationBalance(employeeId, 0m);
            
            return new PayrollCalculationContext(
                baseSalaryRules: new List<BaseSalaryRule> { baseSalaryRule },
                employeeRole: EmployeeRole.Employee,
                totalWorkedHours: 160m,
                overtimeRules: new List<OvertimeRule>(),
                deductionRules: new List<DeductionRule>(),
                productivityMetric: 0m,
                productivityRule: null,
                vacationRule: vacationRule,
                vacationBalance: vacationBalance,
                vacationDaysUsed: 2.5m,
                aguinaldoRule: null,
                aguinaldoBalance: CreateEmployeeAguinaldoBalance(employeeId),
                milestoneRules: new List<MilestoneRule>(),
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone>(),
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: start,
                periodEnd: end
            );
        }

        private static EmployeeVacationBalance CreateEmployeeVacationBalance(Guid employeeId, decimal accruedDays)
        {
            var employee = new Employee(
                "Test Employee",
                "test@test.com",
                EmployeeRole.Employee,
                "hashedPassword123",
                12345);
            
            if (accruedDays > 0)
                employee.AccrueVacationDays(accruedDays);
            
            return employee.VacationBalance;
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
