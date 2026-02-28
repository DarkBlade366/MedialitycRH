using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Features.Payrolls.Aggregates.Payments
{
    public class VacationPayment : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid PayrollId { get; private set; }
        public Guid VacationRuleId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaidAt { get; private set; }

        private VacationPayment() { }

        public VacationPayment(Guid payrollId, Guid vacationRuleId, decimal amount, DateTime paidAt)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            Id = Guid.NewGuid();
            PayrollId = payrollId;
            VacationRuleId = vacationRuleId;
            Amount = amount;
            PaidAt = paidAt;
        }
    }
}
