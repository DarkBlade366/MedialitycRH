using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Payrolls.Aggregates;

namespace Application.Features.Payrolls.Interfaces
{
    public interface IPayrollPdfGenerator
    {
        byte[] Generate(Payroll payroll, string employeeName);
    }
}
