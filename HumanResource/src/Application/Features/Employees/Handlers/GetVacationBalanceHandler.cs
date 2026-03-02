using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Employees.DTOs;
using Application.Features.Employees.Queries;
using Domain.Features.Employees.Interfaces;

namespace Application.Features.Employees.Handlers
{
    public class GetVacationBalanceHandler
    {
        private readonly IEmployeeRepository _repository;

        public GetVacationBalanceHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<VacationBalanceResponse> Handle(
            GetVacationBalanceQuery request, 
            CancellationToken cancellationToken)
        {
            var employee = await _repository.GetByIdAsync(request.EmployeeId);
            
            if (employee == null)
                throw new Exception("Employee not found.");
    
            var balance = employee.VacationBalance;
    
            return new VacationBalanceResponse
            {
                EmployeeId = employee.Id,
                AccruedDays = balance.AccruedDays,
                UsedDays = balance.UsedDays,
                AvailableDays = balance.AvailableDays
            };
        }
    }
}