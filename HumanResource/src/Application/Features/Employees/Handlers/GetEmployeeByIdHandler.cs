using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Employees.Queries;
using Application.Features.Employees.DTOs;
using Domain.Features.Employees.Interfaces;

namespace Application.Features.Employees.Handlers
{
    public class GetEmployeeByIdHandler
    {
        private readonly IEmployeeRepository _repository;

        public GetEmployeeByIdHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetEmployeeByIdResponse> Handle(GetEmployeeByIdQuery query)
        {
            var employee = await _repository.GetByIdWithBalancesAsync(query.Id);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            return new GetEmployeeByIdResponse
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Role = employee.Role.ToString(),
                IsActive = employee.IsActive,
                RedmineUserId = employee.RedmineUserId,
                VacationDaysAvailable = employee.VacationBalance.AvailableDays,
                AguinaldoAvailable = employee.AguinaldoBalance.AccruedAmount,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };
        }
    }
}