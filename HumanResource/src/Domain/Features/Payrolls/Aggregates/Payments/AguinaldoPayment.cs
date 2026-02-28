using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Features.Payrolls.Aggregates.Payments
{
    public class AguinaldoPayment
    {
        public Guid Id { get; private set; }
        public Guid PayrollId { get; private set; }
        public Guid AguinaldoRuleId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaidAt { get; private set; }

        private AguinaldoPayment() { }

        public AguinaldoPayment(Guid payrollId, Guid aguinaldoRuleId, decimal amount, DateTime paidAt)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            Id = Guid.NewGuid();
            PayrollId = payrollId;
            AguinaldoRuleId = aguinaldoRuleId;
            Amount = amount;
            PaidAt = paidAt;
        }
    }
}
