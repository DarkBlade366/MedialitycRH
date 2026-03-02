using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Enums;

namespace Domain.Features.Payrolls.Rules
{
    public class ProductivityRule : PayrollRule
    {
        public decimal MinimumTarget { get; private set; }
        public decimal BonusValue { get; private set; }
        public BonusType BonusType { get; private set; }
        public decimal? MaxBonusCap { get; private set; }
        public decimal FullBonusTarget { get; private set; }

        private ProductivityRule() : base("Productivity Rule") { }

        public ProductivityRule(
        decimal minimumTarget,
        decimal fullBonusTarget,
        decimal bonusValue,
        BonusType bonusType,
        decimal? maxBonusCap = null)
            : base("Productivity Rule")
        {
            if (minimumTarget < 0)
                throw new ArgumentException("Minimum target cannot be negative.");

            if (fullBonusTarget <= minimumTarget)
                throw new ArgumentException("Full bonus target must be greater than minimum target.");

            if (bonusValue <= 0)
                throw new ArgumentException("Bonus value must be greater than zero.");

            MinimumTarget = minimumTarget;
            FullBonusTarget = fullBonusTarget;
            BonusValue = bonusValue;
            BonusType = bonusType;
            MaxBonusCap = maxBonusCap;
        }
    }
}
