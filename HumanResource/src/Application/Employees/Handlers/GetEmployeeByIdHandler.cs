using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Employees.Queries;
using Application.Employees.DTOs;
using Domain.Interfaces;

namespace Application.Employees.Handlers
{
    public class GetEmployeeByIdHandler
    {
        private readonly IEmployeeRepository _repository;

        public GetEmployeeByIdHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmployeeDetailDto> Handle(GetEmployeeByIdQuery query)
        {
            var employee = await _repository.GetByIdAsync(query.Id);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            return new EmployeeDetailDto
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                Role = employee.Role.ToString(),
                IsActive = employee.IsActive,
                RedmineUserId = employee.RedmineUserId,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };
        }
    }
}