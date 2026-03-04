using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payments.Deduction.Queries
{
    public class GetDeductionPaymentsPagedQuery
    {
        public Guid? PayrollId { get; set; }
        public Guid? DeductionRuleId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
