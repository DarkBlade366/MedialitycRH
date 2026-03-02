using System;
using Domain.Common;
using Domain.Features.Payrolls.Enums;

namespace Domain.Features.Payrolls.Entities
{
    public class PayrollComponent : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid RuleId { get; private set; }

        public PayrollComponentType Type { get; private set; }

        public PayrollComponentCategory Category { get; private set; }

        public string Description { get; private set; } = string.Empty;

        public decimal Amount { get; private set; }

        private PayrollComponent() { }

        public PayrollComponent(
            PayrollComponentType type,
            PayrollComponentCategory category,
            string description,
            decimal amount,
            Guid ruleId)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.");

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            Id = Guid.NewGuid();
            Type = type;
            Category = category;
            Description = description;
            Amount = amount;
            RuleId = ruleId;
        }
    }
}
