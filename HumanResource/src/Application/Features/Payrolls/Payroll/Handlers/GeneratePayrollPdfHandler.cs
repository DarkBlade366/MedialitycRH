using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Payroll.Commands;
using Application.Features.Payrolls.Payroll.DTOs;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payroll.Handlers
{
    public class GeneratePayrollPdfHandler
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPayrollPdfGenerator _pdfGenerator;

        public GeneratePayrollPdfHandler(
            IPayrollRepository payrollRepository,
            IEmployeeRepository employeeRepository,
            IPayrollPdfGenerator pdfGenerator)
        {
            _payrollRepository = payrollRepository;
            _employeeRepository = employeeRepository;
            _pdfGenerator = pdfGenerator;
        }

        public async Task<byte[]> Handle(
            GeneratePayrollPdfCommand command,
            CancellationToken cancellationToken)
        {
            var payroll = await _payrollRepository.GetByIdAsync(command.Id);

            if (payroll == null)
                throw new Exception("Payroll not found.");

            var employee = await _employeeRepository.GetByIdAsync(payroll.EmployeeId);

            if (employee == null)
                throw new Exception("Employee not found.");

            var payrollResponse = new PayrollResponse
            {
                Id = payroll.Id,
                PeriodStart = payroll.PeriodStart,
                PeriodEnd = payroll.PeriodEnd,
                GrossAmount = payroll.GrossAmount,
                TotalDeductions = payroll.TotalDeductions,
                NetAmount = payroll.NetAmount,
                Status = payroll.Status.ToString(),
                Components = payroll.Components
                    .Select(c => new PayrollComponentResponse
                    {
                        Type = c.Type.ToString(),
                        Category = c.Category.ToString(),
                        Description = c.Description,
                        Amount = c.Amount
                    })
                    .ToList()
            };

            var pdfBytes = _pdfGenerator.Generate(
                payrollResponse,
                employee.FullName,
                employee.VacationBalance.AvailableDays,
                employee.AguinaldoBalance.AccruedAmount,
                command.Lang);

            return pdfBytes;
        }
    }
}