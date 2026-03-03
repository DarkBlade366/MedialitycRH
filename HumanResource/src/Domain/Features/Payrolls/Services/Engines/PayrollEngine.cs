using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates;
using Domain.Features.Payrolls.Services.Calculators;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Payrolls.Services.Interfaces;

namespace Domain.Features.Payrolls.Services.Engines
{
    public class PayrollEngine
    {
        private readonly IEnumerable<IEarningCalculator> _earningCalculators;
        private readonly IEnumerable<IDeductionCalculator> _deductionCalculators;

        public PayrollEngine(
            IEnumerable<IEarningCalculator> earningCalculators, 
            IEnumerable<IDeductionCalculator> deductionCalculators)
        {
            _earningCalculators = earningCalculators 
                ?? throw new ArgumentNullException(nameof(earningCalculators));
            _deductionCalculators = deductionCalculators 
                ?? throw new ArgumentNullException(nameof(deductionCalculators));
        }

        public Payroll Calculate(Guid employeeId, PayrollCalculationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Console.WriteLine($"[PAYROLL ENGINE] Iniciando cálculo para empleado: {employeeId}");
            Console.WriteLine($"[PAYROLL ENGINE] Período: {context.PeriodStart} - {context.PeriodEnd}");
            Console.WriteLine($"[PAYROLL ENGINE] Earnings calculators: {_earningCalculators.Count()}");
            Console.WriteLine($"[PAYROLL ENGINE] Deduction calculators: {_deductionCalculators.Count()}");

            var payroll = new Payroll(employeeId, context.PeriodStart, context.PeriodEnd);

            Console.WriteLine($"[PAYROLL ENGINE] Ejecutando earnings...");

            foreach (var earning in _earningCalculators)
            {
                Console.WriteLine($"[PAYROLL ENGINE] Ejecutando earning: {earning.GetType().Name}");
                earning.Calculate(payroll, context);
            }

            Console.WriteLine($"[PAYROLL ENGINE] Ejecutando deductions...");

            foreach (var deduction in _deductionCalculators)
            {
                Console.WriteLine($"[PAYROLL ENGINE] Ejecutando deduction: {deduction.GetType().Name}");
                deduction.Calculate(payroll, context);
            }

            payroll.MarkAsCalculated();

            Console.WriteLine($"[PAYROLL ENGINE] Total Earnings: {payroll.GrossAmount}");
            Console.WriteLine($"[PAYROLL ENGINE] Total Deductions: {payroll.TotalDeductions}");
            Console.WriteLine($"[PAYROLL ENGINE] Net Amount: {payroll.NetAmount}");
            Console.WriteLine($"[PAYROLL ENGINE] Cálculo completado");

            return payroll;
        }
    }
}