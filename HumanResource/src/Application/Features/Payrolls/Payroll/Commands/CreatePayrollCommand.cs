using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payroll.Commands
{
    public class CreatePayrollCommand
    {
        public Guid employeeId { get; set; }
        public DateTime periodStart { get; set; }
        public DateTime periodEnd { get; set; }
    }
}