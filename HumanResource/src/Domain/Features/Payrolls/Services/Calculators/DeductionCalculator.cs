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
            Console.WriteLine($"[DEDUCTION] Iniciando cálculo de deducciones");

            var basicSalary = payroll.Components
                .Where(c => c.Type == PayrollComponentType.BaseSalary)
                .Sum(c => c.Amount);

            var totalEarnings = payroll.Components
                .Where(c => c.Category == PayrollComponentCategory.Earning)
                .Sum(c => c.Amount);

            Console.WriteLine($"[DEDUCTION] BasicSalary base: {basicSalary}");
            Console.WriteLine($"[DEDUCTION] TotalEarnings base: {totalEarnings}");

            if (basicSalary <= 0 && totalEarnings <= 0)
            {
                Console.WriteLine($"[DEDUCTION] No hay base para aplicar deducciones");
                return;
            }

            foreach (var rule in context.DeductionRules.Where(r => r.IsActive))
            {
                Console.WriteLine($"[DEDUCTION] Evaluando regla: {rule.Description} - Tipo: {rule.Type} - %: {rule.Percentage}");

                decimal baseAmount = rule.Type switch
                {
                    DeductionType.BasicSalary => basicSalary,
                    DeductionType.TotalEarnings => totalEarnings,
                    _ => 0m
                };

                Console.WriteLine($"[DEDUCTION] Base utilizada para cálculo: {baseAmount}");

                if (baseAmount <= 0)
                {
                    Console.WriteLine($"[DEDUCTION] Base inválida, se omite regla");
                    continue;
                }

                var amount = baseAmount * rule.Percentage;

                Console.WriteLine($"[DEDUCTION] Monto calculado antes de validación: {amount}");

                if (amount <= 0)
                {
                    Console.WriteLine($"[DEDUCTION] Monto <= 0, se omite");
                    continue;
                }

                var component = new PayrollComponent(
                    PayrollComponentType.LegalDeduction,
                    PayrollComponentCategory.Deduction,
                    rule.Description,
                    amount,
                    rule.Id
                );

                payroll.AddComponent(component);

                Console.WriteLine($"[DEDUCTION] Deducción agregada: {component.Description} - {component.Amount}");

                payroll.AddDeductionPayment(rule.Id, amount, DateTime.UtcNow);
            }

            Console.WriteLine($"[DEDUCTION] Finalizó cálculo de deducciones");
        }
    }
}