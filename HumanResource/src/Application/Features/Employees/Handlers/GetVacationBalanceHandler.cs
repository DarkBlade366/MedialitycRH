using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Employees.DTOs;
using Application.Features.Employees.Queries;
using Domain.Features.Employees.Interfaces;
using Application.Common.Interfaces;

namespace Application.Features.Employees.Handlers
{
    public class GetVacationBalanceHandler
    {
        private readonly IEmployeeRepository _repository;
        private readonly ICacheService _cache;

        public GetVacationBalanceHandler(IEmployeeRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<VacationBalanceResponse> Handle(
            GetVacationBalanceQuery request, 
            CancellationToken cancellationToken)
        {
            string cacheKey = $"employee:vacation:{request.EmployeeId}";
            var cached = await _cache.GetAsync<VacationBalanceResponse>(cacheKey);
            if (cached != null)
                return cached;

            var employee = await _repository.GetByIdAsync(request.EmployeeId);
            
            if (employee == null)
                throw new Exception("Employee not found.");
    
            var balance = employee.VacationBalance;
    
            var response = new VacationBalanceResponse
            {
                EmployeeId = employee.Id,
                AccruedDays = balance.AccruedDays,
                UsedDays = balance.UsedDays,
                AvailableDays = balance.AvailableDays
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken); 
            
            return response;
        }
    }
}