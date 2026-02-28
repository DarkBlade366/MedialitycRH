using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Entities;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Projects.Aggregates;

namespace Domain.Features.Payrolls.Services.Context
{
    public class PayrollCalculationContext
    {
        public int TotalWorkedHours { get; }

        //Salary 
        public BaseSalaryRule BaseSalaryRule { get; }

        //Por Hora
        public decimal HourlyRate { get; }

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
        public IReadOnlyCollection<ProjectMilestone> ProjectMilestones { get; }

        //Period
        public DateTime PeriodStart { get; }
        public DateTime PeriodEnd { get; }

        public PayrollCalculationContext(
            BaseSalaryRule? baseSalaryRule,
            decimal hourlyRate,
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
            IReadOnlyCollection<ProjectMilestone> projectMilestones,

            DateTime periodStart,
            DateTime periodEnd)
        {
            if (baseSalaryRule == null)
                throw new ArgumentNullException(nameof(baseSalaryRule));
            
            BaseSalaryRule = baseSalaryRule;
            HourlyRate = hourlyRate;
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
            ProjectMilestones = projectMilestones ?? new List<ProjectMilestone>();

            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
        }
    }
}
