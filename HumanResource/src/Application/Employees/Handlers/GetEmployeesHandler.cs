using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Employees.DTOs;
using Application.Employees.Queries;
using Domain.Interfaces;

namespace Application.Employees.Handlers
{
    public class GetEmployeesHandler
    {
        private readonly IEmployeeRepository _repository;

        public GetEmployeesHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<EmployeeListItemDto>> Handle(GetEmployeesQuery query)
        {
            var (employees, total) =
                await _repository.GetPagedAsync(query.Page, query.PageSize);

            var items = employees.Select(e => new EmployeeListItemDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Email = e.Email,
                Role = e.Role.ToString(),
                IsActive = e.IsActive
            }).ToList();

            var totalPages =
                (int)Math.Ceiling(total / (double)query.PageSize);

            return new PagedResponse<EmployeeListItemDto>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = total,
                TotalPages = totalPages
            };
        }
    }
}