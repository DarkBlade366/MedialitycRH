using System;

namespace Domain.Features.Payrolls.Aggregates.Payments
{
    public class ProjectPayment
    {
        public Guid Id { get; private set; }
        public Guid PayrollId { get; private set; }
        public int RedmineProjectId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaidAt { get; private set; }

        private ProjectPayment() { }

        public ProjectPayment(Guid payrollId, int redmineProjectId, decimal amount, DateTime paidAt)
        {
            if (payrollId == Guid.Empty)
                throw new ArgumentException("PayrollId is required.");

            if (redmineProjectId <= 0)
                throw new ArgumentException("RedmineProjectId must be greater than zero.");

            Id = Guid.NewGuid();
            PayrollId = payrollId;
            RedmineProjectId = redmineProjectId;
            Amount = amount;
            PaidAt = paidAt;
        }
    }
}
