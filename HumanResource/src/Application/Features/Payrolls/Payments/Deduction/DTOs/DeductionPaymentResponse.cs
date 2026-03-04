using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payments.Deduction.DTOs
{
    public class DeductionPaymentResponse
    {
        public Guid Id { get; set; }
        public Guid PayrollId { get; set; }
        public Guid DeductionRuleId { get; set; }
        public decimal Amount { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
