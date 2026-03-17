using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Employees.DTOs;
using Application.Features.Employees.Queries;
using Domain.Features.Employees.Interfaces;
using Application.Common.Interfaces;
using Domain.Features.Employees.Aggregates;

namespace Application.Features.Employees.Handlers
{
    public class GetEmployeesHandler
    {
        private readonly IEmployeeRepository _repository;
        private readonly ICacheService _cache;

        public GetEmployeesHandler(IEmployeeRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<GetEmployeesResponse>> Handle(GetEmployeesQuery query)
        {
            string cacheKey = "employees:all";
            var allEmployees = await _cache.GetAsync<List<Employee>>(cacheKey);

            if (allEmployees == null)
            {
                var (employees, total) = await _repository.GetPagedAsync(1, int.MaxValue);
                allEmployees = employees.ToList();
                await _cache.SetAsync(cacheKey, allEmployees, TimeSpan.FromMinutes(5));
            }

            var totalItems = allEmployees.Count;
            var pagedEmployees = allEmployees
                .OrderBy(e => e.FullName)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var items = pagedEmployees.Select(e => new GetEmployeesResponse
            {
                Id = e.Id,
                RedmineUserId = e.RedmineUserId,
                FullName = e.FullName,
                Email = e.Email,
                Role = e.Role.ToString(),
                IsActive = e.IsActive
            }).ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<GetEmployeesResponse>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}