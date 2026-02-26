using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Payrolls.Interfaces
{
    public interface IPayrollPdfGenerator
    {
        byte[] Generate(Domain.Models.Payroll payroll, string employeeName);
    }
}