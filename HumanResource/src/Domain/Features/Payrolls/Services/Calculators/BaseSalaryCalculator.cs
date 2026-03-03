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
    public class BaseSalaryCalculator : IEarningCalculator
    {
        public void Calculate(Payroll payroll, PayrollCalculationContext context)
        {
            Console.WriteLine($"[BASE SALARY] Iniciando cálculo de salario base");

            Console.WriteLine($"[BASE SALARY] Rol del empleado: {context.EmployeeRole}");

            var rule = context.BaseSalaryRules
                .FirstOrDefault(r => 
                    r.Role == context.EmployeeRole && 
                    r.IsActive);

            if (rule == null)
            {
                Console.WriteLine($"[BASE SALARY] No se encontró regla activa para el rol");
                return;
            }

            Console.WriteLine($"[BASE SALARY] Regla encontrada: {rule.Name} - Monto: {rule.Amount}");

            var component = new PayrollComponent(
                PayrollComponentType.BaseSalary,
                PayrollComponentCategory.Earning,
                rule.Name,
                rule.Amount,
                rule.Id);

            payroll.AddComponent(component);

            Console.WriteLine($"[BASE SALARY] Componente agregado correctamente");
            Console.WriteLine($"[BASE SALARY] Resultado final: {component.Amount}");
            Console.WriteLine($"[BASE SALARY] Finalizó cálculo de salario base");
        }
    }
}