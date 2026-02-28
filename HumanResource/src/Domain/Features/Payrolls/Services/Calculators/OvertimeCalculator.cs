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
    public class OvertimeCalculator : IEarningCalculator
    {
        public void Calculate(Payroll payroll, PayrollCalculationContext context)
        {
            var rule = context.OvertimeRules.FirstOrDefault(r => r.IsActive);

            if (rule == null)
                return;

            if (context.TotalWorkedHours <= rule.StandardHoursPerPeriod)
                return;

            var overtimeHours = context.TotalWorkedHours - rule.StandardHoursPerPeriod;

            var overtimeAmount = overtimeHours * context.HourlyRate * rule.OvertimeMultiplier;

            var component = new PayrollComponent(
                PayrollComponentType.Overtime,
                PayrollComponentCategory.Earning,
                $"Overtime {overtimeHours} hours",
                overtimeAmount);

            payroll.AddComponent(component);
        }
    }
}
