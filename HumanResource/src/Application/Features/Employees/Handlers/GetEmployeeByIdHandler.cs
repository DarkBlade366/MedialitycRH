using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Employees.Queries;
using Application.Features.Employees.DTOs;
using Domain.Features.Employees.Interfaces;
using Application.Common.Interfaces; 

namespace Application.Features.Employees.Handlers
{
    public class GetEmployeeByIdHandler
    {
        private readonly IEmployeeRepository _repository;
        private readonly ICacheService _cache;

        public GetEmployeeByIdHandler(IEmployeeRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<GetEmployeeByIdResponse> Handle(GetEmployeeByIdQuery query)
        {
            string cacheKey = $"employee:{query.Id}";
            var cached = await _cache.GetAsync<GetEmployeeByIdResponse>(cacheKey);
            if (cached != null)
                return cached;

            var employee = await _repository.GetByIdWithBalancesAsync(query.Id);

            if (employee == null)
                throw new KeyNotFoundException("Employee not found");

            var response = new GetEmployeeByIdResponse
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

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return response;
        }
    }
}