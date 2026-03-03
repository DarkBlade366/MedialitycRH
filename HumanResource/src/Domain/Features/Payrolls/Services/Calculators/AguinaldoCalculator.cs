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
    public class AguinaldoCalculator : IEarningCalculator
    {
        public void Calculate(Payroll payroll, PayrollCalculationContext context)
        {
            Console.WriteLine($"[AGUINALDO] Iniciando cálculo de aguinaldo");

            var rule = context.AguinaldoRule;

            if (rule == null || !rule.IsActive)
            {
                Console.WriteLine($"[AGUINALDO] No existe regla activa");
                return;
            }

            Console.WriteLine($"[AGUINALDO] Regla activa encontrada - % mensual: {rule.MonthlyAccrualPercentage} - Mes de pago: {rule.PayMonth}");

            var baseSalaryRule = context.BaseSalaryRules
                .FirstOrDefault(r =>
                    r.Role == context.EmployeeRole &&
                    r.IsActive);

            if (baseSalaryRule == null)
                throw new Exception("No active base salary rule found for employee role.");

            Console.WriteLine($"[AGUINALDO] Salario base encontrado: {baseSalaryRule.Amount}");

            var monthlyAccrual = baseSalaryRule.Amount * rule.MonthlyAccrualPercentage;

            Console.WriteLine($"[AGUINALDO] Monto mensual acumulado: {monthlyAccrual}");

            context.AguinaldoBalance.Accrue(monthlyAccrual);

            var component = new PayrollComponent(
                PayrollComponentType.Aguinaldo,
                PayrollComponentCategory.Accrual,
                "Aguinaldo Accrual",
                monthlyAccrual,
                rule.Id);

            payroll.AddComponent(component);

            Console.WriteLine($"[AGUINALDO] Componente de acumulación agregado correctamente");

            Console.WriteLine($"[AGUINALDO] Mes actual del período: {context.PeriodEnd.Month}");

            if (context.PeriodEnd.Month == rule.PayMonth)
            {
                Console.WriteLine($"[AGUINALDO] Mes de pago alcanzado. Ejecutando pago.");

                var totalToPay = context.AguinaldoBalance.Pay();

                Console.WriteLine($"[AGUINALDO] Total acumulado a pagar: {totalToPay}");

                var component2 = new PayrollComponent(
                    PayrollComponentType.Aguinaldo,
                    PayrollComponentCategory.Earning,
                    "Aguinaldo Payment",
                    totalToPay,
                    rule.Id);

                payroll.AddComponent(component2);

                payroll.AddAguinaldoPayment(rule.Id, totalToPay, DateTime.UtcNow);

                Console.WriteLine($"[AGUINALDO] Pago registrado correctamente");
            }
            else
            {
                Console.WriteLine($"[AGUINALDO] Aún no es mes de pago");
            }

            Console.WriteLine($"[AGUINALDO] Finalizó cálculo de aguinaldo");
        }
    }
}