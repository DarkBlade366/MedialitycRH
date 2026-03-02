using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Entities;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Payrolls.Services.Interfaces;

namespace Domain.Features.Payrolls.Services.Calculators
{
    public class AguinaldoCalculator : IEarningCalculator
    {
        public void Calculate(Payroll payroll, PayrollCalculationContext context)
        {
            var rule = context.AguinaldoRule;

            if (rule == null || !rule.IsActive)
                return;

            var baseSalaryRule = context.BaseSalaryRules
                .FirstOrDefault(r =>
                    r.Role == context.EmployeeRole &&
                    r.IsActive);

            if (baseSalaryRule == null)
                throw new Exception("No active base salary rule found for employee role.");

            var monthlyAccrual = baseSalaryRule.Amount * rule.MonthlyAccrualPercentage;
            context.AguinaldoBalance.Accrue(monthlyAccrual);

            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.Aguinaldo,
                PayrollComponentCategory.Accrual,
                "Aguinaldo Accrual",
                monthlyAccrual,
                rule.Id));

            if (context.PeriodEnd.Month == rule.PayMonth)
            {
                var totalToPay = context.AguinaldoBalance.Pay();

                payroll.AddComponent(new PayrollComponent(
                    PayrollComponentType.Aguinaldo,
                    PayrollComponentCategory.Earning,
                    "Aguinaldo Payment",
                    totalToPay,
                    rule.Id));
                
                payroll.AddAguinaldoPayment(rule.Id, totalToPay, DateTime.Now);
                
            }
        }
    }
}
