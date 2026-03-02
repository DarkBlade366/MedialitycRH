using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Rules;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Payrolls.Services.Interfaces;

namespace Domain.Features.Payrolls.Services.Calculators
{
    public class ProductivityCalculator : IEarningCalculator
    {
        public void Calculate(Payroll payroll, PayrollCalculationContext context)
        {   
            var rule = context.ProductivityRule;

            if (rule == null || !rule.IsActive)
                return;

            var metric = context.ProductivityMetric;
    
            if (metric <= rule.MinimumTarget)
                return;

            decimal proportionalFactor;

            if (metric >= rule.FullBonusTarget)
            {
                proportionalFactor = 1m;
            }
            else
            {
                proportionalFactor =
                    (metric - rule.MinimumTarget) /
                    (rule.FullBonusTarget - rule.MinimumTarget);
            }

            decimal fullBonusAmount = 0;

            switch (rule.BonusType)
            {
                case BonusType.FixedAmount:
                    fullBonusAmount = rule.BonusValue;
                    break;

                case BonusType.Percentage:

                    var gross = payroll.Components
                        .Where(c => c.Category == PayrollComponentCategory.Earning)
                        .Sum(c => c.Amount);

                    fullBonusAmount = gross * (rule.BonusValue / 100m);
                    break;
            }

            var finalAmount = fullBonusAmount * proportionalFactor;

            if (rule.MaxBonusCap.HasValue &&
                finalAmount > rule.MaxBonusCap.Value)
            {
                finalAmount = rule.MaxBonusCap.Value;
            }

            if (finalAmount <= 0)
                return;

            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.ProductivityBonus,
                PayrollComponentCategory.Earning,
                "Proportional Productivity Bonus",
                finalAmount,
                rule.Id));
            
            payroll.AddProductivityPayment(rule.Id, finalAmount, DateTime.UtcNow);
        }
    }
}
