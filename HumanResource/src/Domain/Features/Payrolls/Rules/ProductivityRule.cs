using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Features.Payrolls.Rules
{
    public class ProductivityRule : PayrollRule
    {
        public decimal MinimumTarget { get; private set; }
        public decimal BonusAmount { get; private set; }
        public bool IsPercentage { get; private set; }

        private ProductivityRule() : base("Productivity Rule") { }

        public ProductivityRule(
            decimal minimumTarget,
            decimal bonusAmount,
            bool isPercentage)
            : base("Productivity Rule")
        {
            if (minimumTarget < 0)
                throw new ArgumentException("Minimum target cannot be negative.");

            if (bonusAmount <= 0)
                throw new ArgumentException("Bonus amount must be greater than zero.");

            MinimumTarget = minimumTarget;
            BonusAmount = bonusAmount;
            IsPercentage = isPercentage;
        }
    }
}
