using System;
using System.Linq;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Payrolls.Services.Interfaces;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Projects.Enums;
using Domain.Features.Payrolls.Entities;

namespace Domain.Features.Payrolls.Services.Calculators
{
    public class MilestoneCalculator : IEarningCalculator
    {
        public void Calculate(Payroll payroll, PayrollCalculationContext context)
        {
            var completedMilestones = context.ProjectMilestones
                .Where(m =>
                    m.Status == MilestoneStatus.Completed &&
                    m.CompletedAt.HasValue &&
                    m.CompletedAt.Value >= context.PeriodStart &&
                    m.CompletedAt.Value <= context.PeriodEnd)
                .ToList();

            foreach (var rule in context.MilestoneRules.Where(r => r.IsActive))
            {
                var match = completedMilestones
                    .Any(m => m.RedmineProjectId == rule.RedmineProjectId &&
                            m.Name == rule.MilestoneName);

                if (match)
                {
                    if (!payroll.MilestonePayments.Any(p => p.MilestoneRuleId == rule.Id))
                    {
                        payroll.AddComponent(new PayrollComponent(
                            PayrollComponentType.MilestoneBonus,
                            PayrollComponentCategory.Earning,
                            $"Milestone Bonus - {rule.MilestoneName}",
                            rule.BonusAmount));

                        payroll.AddMilestonePayment(rule.Id, rule.BonusAmount, System.DateTime.Now);
                    }
                }
            }
        }
    }
}