using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Features.Payrolls.Rules
{
    public class VacationRule : PayrollRule
    {
        public decimal AccrualRatePerMonth { get; private set; }
        
        private VacationRule() : base("Vacation Rule") { }

        public VacationRule(decimal accrualRatePerMonth)
            : base("Vacation Rule")
        {
            if (accrualRatePerMonth <= 0)
                throw new ArgumentException("Accrual rate must be greater than zero.");

            AccrualRatePerMonth = accrualRatePerMonth;
        }
    }
}
