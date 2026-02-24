using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Employees.Commands;
using Domain.Interfaces;
using Domain.Models;
namespace Application.Employees.Handlers
{
    public class ChangeEmployeeStatusHandler
    {
        private readonly IEmployeeRepository _employeeRepository;
        public ChangeEmployeeStatusHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task Handle(ChangeEmployeeStatusCommand command)
        {
            var employee = await _employeeRepository.GetByIdAsync(command.Id);
            if (employee == null)
                throw new Exception("Employee not found");
            employee.ChangeStatus(command.IsActive);
            await _employeeRepository.UpdateAsync(employee);
        }
    }
}