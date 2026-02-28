using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Features.Payrolls.Aggregates.Payments
{
    public class OvertimePayment : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid PayrollId { get; private set; }
        public Guid OvertimeRuleId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaidAt { get; private set; }

        private OvertimePayment() { }

        public OvertimePayment(Guid payrollId, Guid overtimeRuleId, decimal amount, DateTime paidAt)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            Id = Guid.NewGuid();
            PayrollId = payrollId;
            OvertimeRuleId = overtimeRuleId;
            Amount = amount;
            PaidAt = paidAt;
        }
    }
}
