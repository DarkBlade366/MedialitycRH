using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Application.Employees.Commands;

namespace Application.Employees.Handlers
{
    public class ChangeEmployeeRedmineUserIdHandler
    {
        private readonly IEmployeeRepository _repository;

        public ChangeEmployeeRedmineUserIdHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(ChangeEmployeeRedmineUserIdCommand command)
        {
            var employee = await _repository.GetByIdAsync(command.Id);

            if (employee == null)
                throw new Exception("Employee not found");
            
            var exists = await _repository
                .ExistsByRedmineUserIdAsync(command.RedmineUserId);

            if (exists)
                throw new InvalidOperationException($"An employee with RedmineUserId {command.RedmineUserId} already exists.");

            employee.SetRedmineUserId(command.RedmineUserId);

            await _repository.UpdateAsync(employee);
        }
    }
}