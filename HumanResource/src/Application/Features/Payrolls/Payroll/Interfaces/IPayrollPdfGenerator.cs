using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Payrolls.Payroll.Interfaces
{
    public interface IPayrollPdfGenerator
    {
        byte[] Generate(Domain.Features.Payrolls.Aggregates.Payroll payroll, string employeeName);
    }
}
