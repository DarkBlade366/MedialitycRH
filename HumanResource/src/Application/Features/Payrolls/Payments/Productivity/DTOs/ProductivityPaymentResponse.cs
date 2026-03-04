using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payments.Productivity.DTOs
{
    public class ProductivityPaymentResponse
    {
        public Guid Id { get; set; }
        public Guid PayrollId { get; set; }
        public Guid ProductivityRuleId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
