using System;
using System.Linq;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Payrolls.Services.Interfaces;

namespace Domain.Features.Payrolls.Services.Calculators
{
    public class MilestoneCalculator : IEarningCalculator
    {
        public void Calculate(Payroll payroll, PayrollCalculationContext context)
        {
            foreach (var rule in context.MilestoneRules.Where(r => r.IsActive))
            {
                var match = context.CompletedMilestones.Any(m =>
                    m.ProjectId == rule.RedmineProjectId &&
                    m.Name == rule.MilestoneName);

                if (match)
                {
                    var alreadyPaid = payroll.MilestonePayments
                        .Any(p => p.MilestoneRuleId == rule.Id);

                    if (!alreadyPaid)
                    {
                        payroll.AddComponent(new PayrollComponent(
                            PayrollComponentType.MilestoneBonus,
                            PayrollComponentCategory.Earning,
                            $"Milestone Bonus - {rule.MilestoneName}",
                            rule.BonusAmount));

                        payroll.AddMilestonePayment(
                            rule.Id,
                            rule.BonusAmount,
                            DateTime.Now);
                    }
                }
            }
        }
    }
}