using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Employees.Commands;
using Domain.Features.Employees.Interfaces;

namespace Application.Features.Employees.Handlers
{
    public class UseVacationHandler
    {
        private readonly IEmployeeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UseVacationHandler(
            IEmployeeRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UseVacationCommand request, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetByIdAsync(request.EmployeeId);

            if (employee == null)
                throw new Exception("Employee not found.");
    
            employee.UseVacationDays(request.Days);
    
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}