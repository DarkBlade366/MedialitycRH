using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Features.Payrolls.Rules
{
    public class DeductionRule : PayrollRule
    {
        public decimal Percentage { get; private set; }
        public bool IsMandatory { get; private set; }

        private DeductionRule() : base("Deduction Rule") { }

        public DeductionRule(string name, decimal percentage, bool isMandatory)
            : base(name)
        {
            if (percentage <= 0 || percentage > 1)
                throw new ArgumentException("Percentage must be between 0 and 1.");

            Percentage = percentage;
            IsMandatory = isMandatory;
        }
    }
}
