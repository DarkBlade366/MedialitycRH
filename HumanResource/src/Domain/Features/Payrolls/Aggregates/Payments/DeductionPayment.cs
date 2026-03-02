using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Features.Payrolls.Aggregates.Payments
{
    public class DeductionPayment : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid PayrollId { get; private set; }
        public Guid DeductionRuleId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime AppliedAt { get; private set; }

        private DeductionPayment() { }

        public DeductionPayment(
            Guid payrollId,
            Guid deductionRuleId,
            decimal amount,
            DateTime appliedAt)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            Id = Guid.NewGuid();
            PayrollId = payrollId;
            DeductionRuleId = deductionRuleId;
            Amount = amount;
            AppliedAt = appliedAt;
        }
    }
}