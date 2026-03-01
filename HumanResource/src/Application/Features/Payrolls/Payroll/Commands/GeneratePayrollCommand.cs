using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Commands
{
    public class GeneratePayrollCommand
    {
        public Guid EmployeeId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
