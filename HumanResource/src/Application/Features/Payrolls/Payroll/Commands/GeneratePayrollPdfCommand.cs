using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payroll.Commands
{
    public class GeneratePayrollPdfCommand
    {
        public Guid Id { get; set; }
        public string Lang { get; set; } = "es";
    }
}