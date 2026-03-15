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

namespace HumanResource.UnitTests.Domain.Features.Payrolls.Calculators
{
    public class MilestoneCalculatorTests
    {
        [Fact]
        public void Calculate_WithCompletedMilestone_ShouldAddBonusComponent()
        {
            // Arrange
            var calculator = new MilestoneCalculator();
            var employeeId = Guid.NewGuid();
            var payroll = CreatePayrollWithBaseSalary(employeeId);
            var context = CreateContextWithCompletedMilestone(employeeId);

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var bonusComponent = payroll.Components.Last(c => c.Type == PayrollComponentType.MilestoneBonus);
            bonusComponent.Category.Should().Be(PayrollComponentCategory.Earning);
            bonusComponent.Amount.Should().Be(2000m);

            payroll.MilestonePayments.Should().HaveCount(1);
            var payment = payroll.MilestonePayments.First();
            payment.Amount.Should().Be(2000m);
            payment.MilestoneRuleId.Should().Be(context.MilestoneRules.First().Id);
        }

        [Fact]
        public void Calculate_WithNoCompletedMilestones_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new MilestoneCalculator();
            var payroll = CreatePayrollWithBaseSalary(Guid.NewGuid());
            var context = CreateContextWithNoCompletedMilestones();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.MilestonePayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithInactiveRule_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new MilestoneCalculator();
            var payroll = CreatePayrollWithBaseSalary(Guid.NewGuid());
            var context = CreateContextWithInactiveRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.MilestonePayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithNoRule_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new MilestoneCalculator();
            var payroll = CreatePayrollWithBaseSalary(Guid.NewGuid());
            var context = CreateContextWithNoRule();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.MilestonePayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithAlreadyPaidMilestone_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new MilestoneCalculator();
            var employeeId = Guid.NewGuid();
            var context = CreateContextWithCompletedMilestone(employeeId);
            var milestoneRuleId = context.MilestoneRules.First().Id;
            var payroll = CreatePayrollWithBaseSalaryAndMilestonePayment(employeeId, milestoneRuleId, 1000m);

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1); 
            payroll.MilestonePayments.Should().HaveCount(1); 
        }

        [Fact]
        public void Calculate_WithNoParticipants_ShouldNotAddComponents()
        {
            // Arrange
            var calculator = new MilestoneCalculator();
            var payroll = CreatePayrollWithBaseSalary(Guid.NewGuid());
            var context = CreateContextWithNoParticipants();

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(1);
            payroll.MilestonePayments.Should().BeEmpty();
        }

        [Fact]
        public void Calculate_WithMultipleParticipants_ShouldAddProportionalBonus()
        {
            // Arrange
            var calculator = new MilestoneCalculator();
            var employeeId = Guid.NewGuid();
            var payroll = CreatePayrollWithBaseSalary(employeeId);
            var context = CreateContextWithMultipleParticipants(employeeId);

            // Act
            calculator.Calculate(payroll, context);

            // Assert
            payroll.Components.Should().HaveCount(2);
            var bonusComponent = payroll.Components.Last(c => c.Type == PayrollComponentType.MilestoneBonus);
            bonusComponent.Amount.Should().Be(1000m); 
            
            payroll.MilestonePayments.Should().HaveCount(1);
            var payment = payroll.MilestonePayments.First();
            payment.Amount.Should().Be(1000m);
        }

        private static Payroll CreatePayrollWithBaseSalary(Guid employeeId)
        {
            var payroll = new Payroll(
                employeeId,
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

        private static Payroll CreatePayrollWithBaseSalaryAndMilestonePayment(Guid employeeId, Guid milestoneRuleId, decimal amount)
        {
            var payroll = CreatePayrollWithBaseSalary(employeeId);
            payroll.AddMilestonePayment(milestoneRuleId, amount, DateTime.UtcNow);
            return payroll;
        }

        private static PayrollCalculationContext CreateContextWithCompletedMilestone(Guid employeeId)
        {
            var milestoneRule = new MilestoneRule(123, "Phase 1", 2000m);
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            
            var milestone = new ProjectMilestone(
                123,
                "Phase 1");
            
            milestone.MarkAsCompleted(DateTime.UtcNow.AddDays(-1));
            
            var participation = new MilestoneParticipation(
                milestone.Id,
                employeeId,
                milestone);
            
            milestone.Participations.Add(participation);
            
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
                milestoneRules: new List<MilestoneRule> { milestoneRule },
                milestoneParticipations: new List<MilestoneParticipation> { participation },
                projectMilestones: new List<ProjectMilestone> { milestone },
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithNoCompletedMilestones()
        {
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var employeeId = Guid.NewGuid();
            
            var milestone = new ProjectMilestone(
                123,
                "Phase 1");
            
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
                projectMilestones: new List<ProjectMilestone> { milestone },
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithInactiveRule()
        {
            var milestoneRule = new MilestoneRule(123, "Phase 1", 2000m);
            milestoneRule.Deactivate();
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var employeeId = Guid.NewGuid();
            
            var milestone = new ProjectMilestone(
                123,
                "Phase 1");
            
            milestone.MarkAsCompleted(DateTime.UtcNow.AddDays(-1));
            
            var participation = new MilestoneParticipation(
                milestone.Id,
                employeeId,
                milestone);
            
            milestone.Participations.Add(participation);
            
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
                milestoneRules: new List<MilestoneRule> { milestoneRule },
                milestoneParticipations: new List<MilestoneParticipation> { participation },
                projectMilestones: new List<ProjectMilestone> { milestone },
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithNoRule()
        {
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var employeeId = Guid.NewGuid();
            
            var milestone = new ProjectMilestone(
                123,
                "Phase 1");
            
            milestone.MarkAsCompleted(DateTime.UtcNow.AddDays(-1));
            
            var participation = new MilestoneParticipation(
                milestone.Id,
                employeeId,
                milestone);
            
            milestone.Participations.Add(participation);
            
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
                milestoneParticipations: new List<MilestoneParticipation> { participation },
                projectMilestones: new List<ProjectMilestone> { milestone },
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithNoParticipants()
        {
            var milestoneRule = new MilestoneRule(123, "Phase 1", 2000m);
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var employeeId = Guid.NewGuid();
            
            var milestone = new ProjectMilestone(
                123,
                "Phase 1");
            
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
                milestoneRules: new List<MilestoneRule> { milestoneRule },
                milestoneParticipations: new List<MilestoneParticipation>(),
                projectMilestones: new List<ProjectMilestone> { milestone },
                projectRules: new List<ProjectRule>(),
                completedProjects: new List<Project>(),
                timeEntries: new List<TimeEntry>(),
                periodStart: new DateTime(2024, 1, 1),
                periodEnd: new DateTime(2024, 1, 31)
            );
        }

        private static PayrollCalculationContext CreateContextWithMultipleParticipants(Guid employeeId)
        {
            var milestoneRule = new MilestoneRule(123, "Phase 1", 2000m);
            var baseSalaryRule = new BaseSalaryRule(EmployeeRole.Employee, 5000m);
            var otherEmployeeId = Guid.NewGuid();
            
            var milestone = new ProjectMilestone(
                123,
                "Phase 1");
            
            milestone.MarkAsCompleted(DateTime.UtcNow.AddDays(-1));
            
            var participation1 = new MilestoneParticipation(
                milestone.Id,
                employeeId,
                milestone);
            
            var participation2 = new MilestoneParticipation(
                milestone.Id,
                otherEmployeeId,
                milestone);
            
            milestone.Participations.Add(participation1);
            milestone.Participations.Add(participation2);
            
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
                milestoneRules: new List<MilestoneRule> { milestoneRule },
                milestoneParticipations: new List<MilestoneParticipation> { participation1, participation2 },
                projectMilestones: new List<ProjectMilestone> { milestone },
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
