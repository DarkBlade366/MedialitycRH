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
            Console.WriteLine($"[OVERTIME] Iniciando cálculo de horas extra");

            var rule = context.OvertimeRules.FirstOrDefault(r => r.IsActive);

            if (rule == null)
            {
                Console.WriteLine($"[OVERTIME] No existe regla activa de overtime");
                return;
            }

            Console.WriteLine($"[OVERTIME] Regla encontrada - StandardHours: {rule.StandardHoursPerPeriod}, HourlyRate: {rule.HourlyRate}, Multiplier: {rule.OvertimeMultiplier}");
            Console.WriteLine($"[OVERTIME] Total horas trabajadas en el período: {context.TotalWorkedHours}");

            if (context.TotalWorkedHours <= rule.StandardHoursPerPeriod)
            {
                Console.WriteLine($"[OVERTIME] No hay horas extra. No se genera componente.");
                return;
            }

            var overtimeHours = context.TotalWorkedHours - rule.StandardHoursPerPeriod;

            Console.WriteLine($"[OVERTIME] Horas extra calculadas: {overtimeHours}");

            var overtimeAmount = overtimeHours * rule.HourlyRate * rule.OvertimeMultiplier;

            Console.WriteLine($"[OVERTIME] Monto de horas extra calculado: {overtimeAmount}");

            var component = new PayrollComponent(
                PayrollComponentType.Overtime,
                PayrollComponentCategory.Earning,
                $"Overtime {overtimeHours} hours",
                overtimeAmount,
                rule.Id);

            payroll.AddComponent(component);
            payroll.AddOvertimePayment(rule.Id, overtimeAmount, DateTime.UtcNow);

            Console.WriteLine($"[OVERTIME] Componente agregado correctamente");
            Console.WriteLine($"[OVERTIME] Finalizó cálculo de horas extra");
        }
    }
}