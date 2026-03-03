using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payroll.DTOs
{
    public class PayrollResponse
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetAmount { get; set; }

        public List<PayrollComponentResponse>? Components { get; set; }
    }
}   