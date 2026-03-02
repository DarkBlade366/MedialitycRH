using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Payrolls.Services.Interfaces;

namespace Domain.Features.Payrolls.Services.Calculators
{
    public class DeductionCalculator : IDeductionCalculator
    {
        public void Calculate(Payroll payroll, PayrollCalculationContext context)
        {
            var basicSalary = payroll.Components
                .Where(c => c.Type == PayrollComponentType.BaseSalary)
                .Sum(c => c.Amount);

            var totalEarnings = payroll.Components
                .Where(c => c.Category == PayrollComponentCategory.Earning)
                .Sum(c => c.Amount);

            if (basicSalary <= 0 && totalEarnings <= 0)
                return;

            foreach (var rule in context.DeductionRules.Where(r => r.IsActive))
            {
                decimal baseAmount = rule.Type switch
                {
                    DeductionType.BasicSalary => basicSalary,
                    DeductionType.TotalEarnings => totalEarnings,
                    _ => 0m
                };

                if (baseAmount <= 0)
                    continue;

                var amount = baseAmount * rule.Percentage;

                if (amount <= 0)
                    continue;

                payroll.AddComponent(new PayrollComponent(
                    PayrollComponentType.LegalDeduction,
                    PayrollComponentCategory.Deduction,
                    rule.Description,
                    amount,
                    rule.Id
                ));

                payroll.AddDeductionPayment(rule.Id, amount, DateTime.UtcNow);
            }
        }
    }
}
