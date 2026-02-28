using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Employees.Interfaces;
using Application.Features.Employees.Queries;
using Application.Features.Employees.DTOs;
using Application.Common.Interfaces;

namespace Application.Features.Employees.Handlers
{
    public class GetEmployeeByRedmineUserIdHandler
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GetEmployeeByRedmineUserIdHandler(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetEmployeeByIdResponse> Handle(
            GetEmployeeByRedmineUserIdQuery query, 
            CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByRedmineUserIdAsync(query.RedmineUserId);

            if (employee == null)
                throw new KeyNotFoundException($"Employee with RedmineUserId {query.RedmineUserId} not found.");

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