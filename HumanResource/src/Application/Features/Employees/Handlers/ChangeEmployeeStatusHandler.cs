using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Threading.Tasks;
using Domain.Features.Employees.Interfaces;
using Application.Features.Employees.Commands;
using Application.Common.Interfaces;

namespace Application.Features.Employees.Handlers
{
    public class ChangeEmployeeStatusHandler
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cache;

        public ChangeEmployeeStatusHandler(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            ICacheService cache)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task Handle(
            ChangeEmployeeStatusCommand command, 
            CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(command.Id);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            employee.ChangeStatus(command.IsActive);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
                        
            await _cache.RemoveAsync($"employee:{employee.Id}", cancellationToken); 
            await _cache.RemoveAsync($"employee:redmine:{employee.RedmineUserId}", cancellationToken); 
            await _cache.RemoveAsync("employees:all", cancellationToken);
        }
    }
}