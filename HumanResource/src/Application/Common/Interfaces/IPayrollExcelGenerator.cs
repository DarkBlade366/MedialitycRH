using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.DTOs;

namespace Application.Common.Interfaces
{
    public interface IPayrollExcelGenerator
    {
        byte[] Generate(
            PayrollResponse payroll,
            string employeeName,
            decimal vacationAvailableDays,
            decimal aguinaldoAccruedAmount,
            string language);
    }
}