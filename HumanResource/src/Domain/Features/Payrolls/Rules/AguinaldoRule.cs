using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Features.Payrolls.Rules
{
    public class AguinaldoRule : PayrollRule
    {
        public decimal MonthlyAccrualPercentage { get; private set; }
        public int PayMonth { get; private set; }

        private AguinaldoRule() : base("Aguinaldo Rule") { }

        public AguinaldoRule(decimal monthlyAccrualPercentage, int payMonth)
            : base("Aguinaldo Rule")
        {
            if (monthlyAccrualPercentage <= 0)
                throw new ArgumentException("Accrual percentage must be positive.");

            MonthlyAccrualPercentage = monthlyAccrualPercentage;
            PayMonth = payMonth;
        }
    }
}
