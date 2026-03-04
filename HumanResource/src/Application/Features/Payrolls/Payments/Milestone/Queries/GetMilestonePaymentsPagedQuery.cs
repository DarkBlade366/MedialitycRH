using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payments.Milestone.Queries
{
    public class GetMilestonePaymentsPagedQuery
    {
        public Guid? PayrollId { get; set; }
        public Guid? MilestoneRuleId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
