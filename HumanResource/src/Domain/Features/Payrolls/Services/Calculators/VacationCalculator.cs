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
    public class VacationCalculator : IEarningCalculator
    {
        public void Calculate(Payroll payroll, PayrollCalculationContext context)
        {
            var rule = context.VacationRule;
            var balance = context.VacationBalance;

            if (rule == null || balance == null)
                return;

            // Accrual mensual
            balance.Accrue(rule.AccrualRatePerMonth);

            // Pago por uso
            if (rule.PayVacationOnUse && context.VacationDaysUsed > 0)
            {   
                var baseSalaryRule = context.BaseSalaryRules
                    .FirstOrDefault(r =>
                        r.Role == context.EmployeeRole &&
                        r.IsActive);

                if (baseSalaryRule == null)
                    throw new Exception("No active base salary rule found for employee role.");

                var dailyRate = baseSalaryRule.Amount / 30m;
                var amount = dailyRate * context.VacationDaysUsed;

                payroll.AddComponent(new PayrollComponent(
                    PayrollComponentType.VacationPay,
                    PayrollComponentCategory.Earning,
                    "Vacation Payment",
                    amount,
                    rule.Id));

                balance.Use(context.VacationDaysUsed);
            }
        }
    }
}
