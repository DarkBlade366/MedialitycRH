using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payroll.Queries
{
    public class GetPayrollPagedQuery
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Status { get; set; }
        public Guid? EmployeeId { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
    }
}