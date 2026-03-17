using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Employees.Commands;
using Domain.Features.Employees.Interfaces;
using Domain.Features.Employees.Aggregates;
using Domain.Common.Security;

namespace Application.Features.Employees.Handlers
{
    public class CreateEmployeeHandler
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public CreateEmployeeHandler(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Guid> Handle(
            CreateEmployeeCommand command,
            CancellationToken cancellationToken)
        {
            var normalizedEmail = command.Email.Trim().ToLowerInvariant();

            var redmineExists = await _employeeRepository
                .ExistsByRedmineUserIdAsync(command.RedmineUserId);

            if (redmineExists)
                throw new InvalidOperationException(
                    $"An employee with RedmineUserId {command.RedmineUserId} already exists.");

            var emailExists = await _employeeRepository
                .ExistsByEmailAsync(normalizedEmail);

            if (emailExists)
                throw new InvalidOperationException("Email already in use.");

            var passwordHash = PasswordHasher.Hash(command.Password);

            var employee = new Employee(
                command.FullName,
                normalizedEmail,
                command.Role,
                passwordHash,
                command.RedmineUserId
            );

            await _employeeRepository.AddAsync(employee);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cache.RemoveAsync("employees:all", cancellationToken);

            return employee.Id;
        }
    }
}