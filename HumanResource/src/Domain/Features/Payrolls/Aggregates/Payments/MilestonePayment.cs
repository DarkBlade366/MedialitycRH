using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Features.Payrolls.Aggregates.Payments
{
    public class MilestonePayment : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid PayrollId { get; private set; }
        public Guid MilestoneRuleId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaidAt { get; private set; }
    
        private MilestonePayment() { }
    
        public MilestonePayment(Guid payrollId, Guid milestoneRuleId, decimal amount, DateTime paidAt)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");
    
            Id = Guid.NewGuid();
            PayrollId = payrollId;
            MilestoneRuleId = milestoneRuleId;
            Amount = amount;
            PaidAt = paidAt;
        }
    }
}
