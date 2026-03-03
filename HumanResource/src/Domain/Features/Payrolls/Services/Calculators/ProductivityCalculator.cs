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
            Console.WriteLine($"[PRODUCTIVITY] Iniciando cálculo de bono de productividad");

            var rule = context.ProductivityRule;

            if (rule == null || !rule.IsActive)
            {
                Console.WriteLine($"[PRODUCTIVITY] No existe regla activa");
                return;
            }

            Console.WriteLine($"[PRODUCTIVITY] Regla activa encontrada - Tipo: {rule.BonusType}, Valor: {rule.BonusValue}");

            var metric = context.ProductivityMetric;

            Console.WriteLine($"[PRODUCTIVITY] Métrica obtenida: {metric}");
            Console.WriteLine($"[PRODUCTIVITY] MinimumTarget: {rule.MinimumTarget}, FullBonusTarget: {rule.FullBonusTarget}");

            if (metric <= rule.MinimumTarget)
            {
                Console.WriteLine($"[PRODUCTIVITY] No supera el mínimo requerido. No se genera bono.");
                return;
            }

            decimal proportionalFactor;

            if (metric >= rule.FullBonusTarget)
            {
                proportionalFactor = 1m;
                Console.WriteLine($"[PRODUCTIVITY] Meta completa alcanzada. Factor proporcional: 1");
            }
            else
            {
                proportionalFactor =
                    (metric - rule.MinimumTarget) /
                    (rule.FullBonusTarget - rule.MinimumTarget);

                Console.WriteLine($"[PRODUCTIVITY] Factor proporcional calculado: {proportionalFactor}");
            }

            decimal fullBonusAmount = 0;

            switch (rule.BonusType)
            {
                case BonusType.FixedAmount:
                    fullBonusAmount = rule.BonusValue;
                    Console.WriteLine($"[PRODUCTIVITY] Bono fijo aplicado: {fullBonusAmount}");
                    break;

                case BonusType.Percentage:

                    var gross = payroll.Components
                        .Where(c => c.Category == PayrollComponentCategory.Earning)
                        .Sum(c => c.Amount);

                    Console.WriteLine($"[PRODUCTIVITY] Gross earnings actual: {gross}");

                    fullBonusAmount = gross * (rule.BonusValue / 100m);

                    Console.WriteLine($"[PRODUCTIVITY] Bono porcentual calculado: {fullBonusAmount}");
                    break;
            }

            var finalAmount = fullBonusAmount * proportionalFactor;

            Console.WriteLine($"[PRODUCTIVITY] Monto después de aplicar factor proporcional: {finalAmount}");

            if (rule.MaxBonusCap.HasValue &&
                finalAmount > rule.MaxBonusCap.Value)
            {
                Console.WriteLine($"[PRODUCTIVITY] Aplicando tope máximo: {rule.MaxBonusCap.Value}");
                finalAmount = rule.MaxBonusCap.Value;
            }

            if (finalAmount <= 0)
            {
                Console.WriteLine($"[PRODUCTIVITY] Monto final es 0 o negativo. No se genera componente.");
                return;
            }

            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.ProductivityBonus,
                PayrollComponentCategory.Earning,
                "Proportional Productivity Bonus",
                finalAmount,
                rule.Id));

            payroll.AddProductivityPayment(rule.Id, finalAmount, DateTime.UtcNow);

            Console.WriteLine($"[PRODUCTIVITY] Bono agregado correctamente por monto: {finalAmount}");
            Console.WriteLine($"[PRODUCTIVITY] Finalizó cálculo de productividad");
        }
    }
}