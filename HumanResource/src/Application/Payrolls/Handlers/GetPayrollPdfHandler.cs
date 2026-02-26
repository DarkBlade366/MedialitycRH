using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Application.Payrolls.Interfaces;
using Application.Payrolls.Queries;

namespace Application.Payrolls.Handlers
{
    public class GetPayrollPdfHandler
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPayrollPdfGenerator _pdfGenerator;

        public GetPayrollPdfHandler(IPayrollRepository payrollRepository, IEmployeeRepository employeeRepository,  IPayrollPdfGenerator pdfGenerator)
        {
            _payrollRepository = payrollRepository;
            _employeeRepository = employeeRepository;
            _pdfGenerator = pdfGenerator;
        }

        public async Task<GetPayrollPdfResult> HandleAsync(GetPayrollPdfQuery query, CancellationToken ct)
        {
            var payroll = await _payrollRepository.GetByIdAsync(query.PayrollId)
                ?? throw new Exception("Payroll not found.");

            var employee = await _employeeRepository
                .GetByIdAsync(payroll.EmployeeId)
                ?? throw new Exception("Employee not found.");

            var pdfBytes = _pdfGenerator.Generate(payroll, employee.FullName);

            var fileName = $"Payroll_{payroll.PeriodFrom:yyyyMM}.pdf";

            return new GetPayrollPdfResult{
                FileBytes = pdfBytes,
                FileName = fileName
            };
        }
    }
}