using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Entities;
using Domain.Features.Employees.Enums;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Projects.Aggregates;
using Domain.Features.TimeEntries.Aggregates;

namespace Domain.Features.Payrolls.Services.Context
{
    public class PayrollCalculationContext
    {
        public decimal TotalWorkedHours { get; init; }

        //Salary 
        public IReadOnlyCollection<BaseSalaryRule> BaseSalaryRules { get; init; }
        public EmployeeRole EmployeeRole { get; init; }

        //Overtime
        public IReadOnlyCollection<OvertimeRule> OvertimeRules { get; init; }

        //Deductions
        public IReadOnlyCollection<DeductionRule> DeductionRules { get; init; }

        //Productivity
        public decimal ProductivityMetric { get; init; }
        public ProductivityRule? ProductivityRule { get; init; }

        //Vacation
        public VacationRule? VacationRule { get; init; }
        public EmployeeVacationBalance? VacationBalance { get; init; }
        public decimal VacationDaysUsed { get; init; }

        //Aguinaldo
        public AguinaldoRule? AguinaldoRule { get; init; }
        public EmployeeAguinaldoBalance AguinaldoBalance { get; init; }

        //Milestones
        public IReadOnlyCollection<MilestoneRule> MilestoneRules { get; init; }
        public IReadOnlyCollection<MilestoneParticipation> MilestoneParticipations { get; init; }
        public IReadOnlyCollection<ProjectMilestone> ProjectMilestones { get; init; }

        //Project Payments
        public IReadOnlyCollection<ProjectRule> ProjectRules { get; init; }
        public IReadOnlyCollection<Project> CompletedProjects { get; init; }
        public IReadOnlyCollection<TimeEntry> TimeEntries { get; init; }

        //Period
        public DateTime PeriodStart { get; init; }
        public DateTime PeriodEnd { get; init; }

        public PayrollCalculationContext(
            IReadOnlyCollection<BaseSalaryRule> baseSalaryRules,
            EmployeeRole employeeRole,

            decimal totalWorkedHours,

            IReadOnlyCollection<OvertimeRule> overtimeRules,
            IReadOnlyCollection<DeductionRule> deductionRules,

            decimal productivityMetric,
            ProductivityRule? productivityRule,

            VacationRule? vacationRule,
            EmployeeVacationBalance? vacationBalance,
            decimal vacationDaysUsed,

            AguinaldoRule? aguinaldoRule,
            EmployeeAguinaldoBalance aguinaldoBalance,

            IReadOnlyCollection<MilestoneRule> milestoneRules,
            IReadOnlyCollection<MilestoneParticipation> milestoneParticipations,
            IReadOnlyCollection<ProjectMilestone> projectMilestones,

            IReadOnlyCollection<ProjectRule> projectRules,
            IReadOnlyCollection<Project> completedProjects,
            IReadOnlyCollection<TimeEntry> timeEntries,

            DateTime periodStart,
            DateTime periodEnd)
        {            
            BaseSalaryRules = baseSalaryRules ?? new List<BaseSalaryRule>();
            EmployeeRole = employeeRole;

            TotalWorkedHours = totalWorkedHours;

            OvertimeRules = overtimeRules ?? new List<OvertimeRule>();
            DeductionRules = deductionRules ?? new List<DeductionRule>();

            ProductivityMetric = productivityMetric;
            ProductivityRule = productivityRule;

            VacationRule = vacationRule;
            VacationBalance = vacationBalance;
            VacationDaysUsed = vacationDaysUsed;

            AguinaldoRule = aguinaldoRule;
            AguinaldoBalance = aguinaldoBalance;

            MilestoneRules = milestoneRules ?? new List<MilestoneRule>();
            MilestoneParticipations = milestoneParticipations ?? new List<MilestoneParticipation>();
            ProjectMilestones = projectMilestones ?? new List<ProjectMilestone>();

            ProjectRules = projectRules ?? new List<ProjectRule>();
            CompletedProjects = completedProjects ?? new List<Project>();
            TimeEntries = timeEntries ?? new List<TimeEntry>();

            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
        }
    }
}
