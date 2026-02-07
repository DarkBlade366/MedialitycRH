using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Employees;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Employees
{
    public class CreateEmployeeHandler
    {
        private readonly IEmployeeRepository _employeeRepository;
        public CreateEmployeeHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<Guid> Handle(CreateEmployeeCommand command)
        {
            var employee = new Employee(
                command.FullName,
                command.Email,
                command.Role
            );

            await _employeeRepository.AddAsync(employee);

            return employee.Id;
        }
    }
}