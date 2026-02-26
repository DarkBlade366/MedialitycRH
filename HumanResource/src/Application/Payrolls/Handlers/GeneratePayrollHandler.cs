using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Payrolls.Commands;
using Domain.Interfaces;
using Domain.Models;
using Domain.Services;

namespace Application.Payrolls.Handlers
{
    public class GeneratePayrollHandler
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly IPayrollRepository _payrollRepository;
        private readonly IPayrollEngine _payrollEngine;

        public GeneratePayrollHandler(IEmployeeRepository employeeRepository, ITimeEntryRepository timeEntryRepository, IPayrollRepository payrollRepository, IPayrollEngine payrollEngine)
        {
            _employeeRepository = employeeRepository;
            _timeEntryRepository = timeEntryRepository;
            _payrollRepository = payrollRepository;
            _payrollEngine = payrollEngine;
        }

        public async Task<Guid> Handle(GeneratePayrollCommand command)
        {
            //Validar duplicado
            if (await _payrollRepository.ExistsAsync(
                    command.EmployeeId,
                    command.From,
                    command.To))
            {
                throw new Exception("Payroll already exists for this period.");
            }

            //Obtener empleado
            var employee = await _employeeRepository
                .GetByIdAsync(command.EmployeeId)
                ?? throw new Exception("Employee not found.");

            //Obtener horas del período
            var timeEntries = await _timeEntryRepository
                .GetByEmployeeAndPeriodAsync(
                    command.EmployeeId,
                    command.From,
                    command.To);

            if (!timeEntries.Any())
                throw new Exception("No time entries found.");

            //Generar payroll
            var payroll = await _payrollEngine.GenerateAsync(
                employee,
                command.From,
                command.To,
                timeEntries);

            //Guardar
            await _payrollRepository.AddAsync(payroll);

            return payroll.Id;
        }
    }
}