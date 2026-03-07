using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payments.Project.DTOs
{
    public class ProjectPaymentResponse
    {
        public Guid Id { get; set; }
        public Guid PayrollId { get; set; }
        public int RedmineProjectId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
