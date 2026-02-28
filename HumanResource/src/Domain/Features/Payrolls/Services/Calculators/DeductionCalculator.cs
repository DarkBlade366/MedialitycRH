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
            var gross = payroll.Components
                .Where(c => c.Category == PayrollComponentCategory.Earning)
                .Sum(c => c.Amount);

            foreach (var rule in context.DeductionRules.Where(r => r.IsActive))
            {
                var amount = gross * rule.Percentage;

                if (amount <= 0)
                    continue;

                var component = new PayrollComponent(
                    PayrollComponentType.LegalDeduction,
                    PayrollComponentCategory.Deduction,
                    rule.Name,
                    amount);

                payroll.AddComponent(component);
            }
        }
    }
}
