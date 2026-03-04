using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payments.Vacation.DTOs
{
    public class VacationPaymentResponse
    {
        public Guid Id { get; set; }
        public Guid PayrollId { get; set; }
        public Guid VacationRuleId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
