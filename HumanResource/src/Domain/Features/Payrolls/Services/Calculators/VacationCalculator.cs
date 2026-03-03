using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Aggregates.Payments;
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
            Console.WriteLine($"[VACATION] Iniciando cálculo de pago por vacaciones");

            var rule = context.VacationRule;

            if (rule == null)
            {
                Console.WriteLine($"[VACATION] No existe regla de vacaciones");
                return;
            }

            if (context.VacationDaysUsed <= 0)
            {
                Console.WriteLine($"[VACATION] No hay días de vacaciones usados");
                return;
            }

            Console.WriteLine($"[VACATION] Días de vacaciones usados: {context.VacationDaysUsed}");

            var baseSalaryRule = context.BaseSalaryRules
                .FirstOrDefault(r =>
                        r.Role == context.EmployeeRole &&
                        r.IsActive);
                        
            if (baseSalaryRule == null)
                throw new Exception("No active base salary rule found for employee role.");

            Console.WriteLine($"[VACATION] Regla salario base encontrada: {baseSalaryRule.Name} - {baseSalaryRule.Amount}");

            var dailyRate = baseSalaryRule.Amount / 30m;

            Console.WriteLine($"[VACATION] Tarifa diaria calculada: {dailyRate}");

            var amount = dailyRate * context.VacationDaysUsed;

            Console.WriteLine($"[VACATION] Monto total de vacaciones calculado: {amount}");

            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.VacationPay,
                PayrollComponentCategory.Earning,
                "Vacation Payment",
                amount,
                rule.Id));

            payroll.AddVacationPayment(rule.Id, amount, DateTime.UtcNow);

            Console.WriteLine($"[VACATION] Pago de vacaciones registrado correctamente");
            Console.WriteLine($"[VACATION] Finalizó cálculo de vacaciones");
        }
    }
}