using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Payrolls.Payroll.Commands;
using Application.Features.Payrolls.Payroll.Handlers;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Services
{
    public class MonthlyPayrollService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPayrollRepository _payrollRepository;
        private readonly CreatePayrollHandler _createPayrollHandler;

        public MonthlyPayrollService(
            IEmployeeRepository employeeRepository,
            IPayrollRepository payrollRepository,
            CreatePayrollHandler createPayrollHandler)
        {
            _employeeRepository = employeeRepository;
            _payrollRepository = payrollRepository;
            _createPayrollHandler = createPayrollHandler;
        }

        public async Task<MonthlyPayrollExecutionResult> GenerateForAllActiveEmployeesAsync(
            DateTime periodStart,
            DateTime periodEnd,
            CancellationToken cancellationToken = default)
        {
            var employees = await _employeeRepository.GetAllActiveAsync();
            var result = new MonthlyPayrollExecutionResult
            {
                TotalEmployees = employees.Count
            };

            foreach (var employee in employees)
            {
                var alreadyExists = await _payrollRepository.ExistsOverlappingPayroll(
                    employee.Id,
                    periodStart,
                    periodEnd);

                if (alreadyExists)
                {
                    result.SkippedPayrolls++;
                    continue;
                }

                try
                {
                    await _createPayrollHandler.Handle(
                        new CreatePayrollCommand
                        {
                            employeeId = employee.Id,
                            periodStart = periodStart,
                            periodEnd = periodEnd
                        },
                        cancellationToken);

                    result.CreatedPayrolls++;
                }
                catch (Exception ex)
                {
                    result.FailedPayrolls++;
                    result.Errors.Add($"Employee {employee.Id}: {ex.Message}");
                }
            }

            return result;
        }
    }
}
