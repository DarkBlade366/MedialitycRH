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
        private readonly IReadOnlyCollection<IEarningCalculator> _earningCalculators;
        private readonly IReadOnlyCollection<IDeductionCalculator> _deductionCalculators;

        public PayrollEngine(IReadOnlyCollection<IEarningCalculator> earningCalculators, IReadOnlyCollection<IDeductionCalculator> deductionCalculators)
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

            var payroll = new Payroll(employeeId, context.PeriodStart, context.PeriodEnd);

            foreach (var earning in _earningCalculators)
            {
                earning.Calculate(payroll, context);
            }

            foreach (var deduction in _deductionCalculators)
            {
                deduction.Calculate(payroll, context);
            }

            payroll.MarkAsCalculated();

            return payroll;
        }
    }
}
