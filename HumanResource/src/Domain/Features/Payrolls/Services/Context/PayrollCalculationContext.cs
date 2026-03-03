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

namespace Domain.Features.Payrolls.Services.Context
{
    public class PayrollCalculationContext
    {
        public int TotalWorkedHours { get; init; }

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

        //Period
        public DateTime PeriodStart { get; init; }
        public DateTime PeriodEnd { get; init; }

        public PayrollCalculationContext(
            IReadOnlyCollection<BaseSalaryRule> baseSalaryRules,
            EmployeeRole employeeRole,

            int totalWorkedHours,

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

            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
        }
    }
}
