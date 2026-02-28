using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Features.Payrolls.Rules
{
    public class BaseSalaryRule : PayrollRule
    {
        public decimal Amount { get; private set; }

        private BaseSalaryRule() : base("Base Salary Rule") { }

        public BaseSalaryRule(string name, decimal amount)
            : base(name)
        {
            if (amount <= 0)
                throw new ArgumentException("Base salary must be greater than zero.");

            Amount = amount;
        }
    }
}
