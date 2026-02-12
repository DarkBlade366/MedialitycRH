using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Employees.Commands;
using Domain.Interfaces;

namespace Application.Employees.Handlers
{
    public class UpdateEmployeeHandler
    {
        private readonly IEmployeeRepository _employeeRepository;

        public UpdateEmployeeHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task Handle(UpdateEmployeeCommand command)
        {
            var employee = await _employeeRepository.GetByIdAsync(command.Id);

            if (employee == null)
                throw new Exception("Employee not found");

            employee.Update(
                command.FullName,
                command.Email,
                command.Role
            );

            await _employeeRepository.UpdateAsync(employee);
        }
    }
}