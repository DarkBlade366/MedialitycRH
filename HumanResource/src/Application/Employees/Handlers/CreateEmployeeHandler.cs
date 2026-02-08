using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Employees.Commands;
using Domain.Interfaces;
using Domain.Models;
using Domain.Common.Security;

namespace Application.Employees.Handlers
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
            var passwordHash = PasswordHasher.Hash(command.Password);

            var employee = new Employee(
                command.FullName,
                command.Email,
                command.Role,
                passwordHash
            );

            await _employeeRepository.AddAsync(employee);

            return employee.Id;
        }
    }
}