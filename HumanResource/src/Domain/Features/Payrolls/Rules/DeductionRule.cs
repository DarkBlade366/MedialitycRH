using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Enums;

namespace Domain.Features.Payrolls.Rules
{
    public class DeductionRule : PayrollRule
    {
        public decimal Percentage { get; private set; }

        public string Description { get; private set; } = string.Empty;

        public DeductionType Type { get; private set; }

        private DeductionRule() : base("Deduction Rule") { }

        public DeductionRule(string name, decimal percentage, string description, DeductionType type)
            : base(name)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.");
                
            if (percentage <= 0 || percentage > 1)
                throw new ArgumentException("Percentage must be between 0 and 1.");

            Percentage = percentage;
            Description = description;
            Type = type;
        }
    }
}
