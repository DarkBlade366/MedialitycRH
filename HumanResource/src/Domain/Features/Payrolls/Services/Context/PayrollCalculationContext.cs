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
        public int TotalWorkedHours { get; }

        //Salary 
        public IReadOnlyCollection<BaseSalaryRule> BaseSalaryRules { get; }
        public EmployeeRole EmployeeRole { get; }

        //Overtime
        public IReadOnlyCollection<OvertimeRule> OvertimeRules { get; }

        //Deductions
        public IReadOnlyCollection<DeductionRule> DeductionRules { get; }

        //Productivity
        public decimal ProductivityMetric { get; }
        public ProductivityRule? ProductivityRule { get; }

        //Vacation
        public VacationRule? VacationRule { get; }
        public EmployeeVacationBalance? VacationBalance { get; }
        public decimal VacationDaysUsed { get; }

        //Aguinaldo
        public AguinaldoRule? AguinaldoRule { get; }
        public EmployeeAguinaldoBalance AguinaldoBalance { get; }

        //Milestones
        public IReadOnlyCollection<MilestoneRule> MilestoneRules { get; }
        public IReadOnlyCollection<MilestoneParticipation> MilestoneParticipations { get; }
        public IReadOnlyCollection<ProjectMilestone> ProjectMilestones { get; }

        //Period
        public DateTime PeriodStart { get; }
        public DateTime PeriodEnd { get; }

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
