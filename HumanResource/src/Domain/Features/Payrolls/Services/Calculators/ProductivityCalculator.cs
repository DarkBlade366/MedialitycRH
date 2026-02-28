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

            if (context.ProductivityMetric < rule.MinimumTarget)
                return;

            decimal amount;

            if (rule.IsPercentage)
            {
                var gross = payroll.Components
                    .Where(c => c.Category == PayrollComponentCategory.Earning)
                    .Sum(c => c.Amount);

                amount = gross * rule.BonusAmount;
            }
            else
            {
                amount = rule.BonusAmount;
            }

            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.ProductivityBonus,
                PayrollComponentCategory.Earning,
                "Productivity Bonus",
                amount));
        }
    }
}
