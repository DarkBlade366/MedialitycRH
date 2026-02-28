using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Commands
{
    public class ApprovePayrollCommand
    {
        public Guid PayrollId { get; set; }
    }
}
