using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Services.Context;
using Domain.Features.Payrolls.Aggregates;

namespace Domain.Features.Payrolls.Services.Interfaces
{
    public interface IDeductionCalculator
    {
        void Calculate(Payroll payroll, PayrollCalculationContext context);
    }
}
