using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payments.Project.Queries
{
    public class GetProjectPaymentsPagedQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? PayrollId { get; set; }
        public int? RedmineProjectId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
