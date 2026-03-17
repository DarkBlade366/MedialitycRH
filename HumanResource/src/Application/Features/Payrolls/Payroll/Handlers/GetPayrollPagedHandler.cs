using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Payroll.DTOs;
using Application.Features.Payrolls.Payroll.Queries;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Interfaces;
using PayrollAgg = Domain.Features.Payrolls.Aggregates.Payroll;

namespace Application.Features.Payrolls.Payroll.Handlers
{
    public class GetPayrollPagedHandler
    {
        private readonly IPayrollRepository _repository;
        private readonly ICacheService _cache;

        public GetPayrollPagedHandler(IPayrollRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<PayrollResponse>> HandleAsync(GetPayrollPagedQuery query)
        {
            string cacheKey = "payrolls:all";
            var payrolls = await _cache.GetAsync<List<PayrollAgg>>(cacheKey);
            
            if (payrolls == null)
            {
                payrolls = (await _repository.GetAllAsync())?.ToList() ?? new List<PayrollAgg>();
                await _cache.SetAsync(cacheKey, payrolls, TimeSpan.FromMinutes(10));
            }

            var filtered = payrolls.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                if (Enum.TryParse<PayrollStatus>(query.Status, true, out var parsedStatus))
                {
                    filtered = filtered.Where(p => p.Status == parsedStatus);
                }
            }

            if (query.EmployeeId.HasValue)
            {
                filtered = filtered.Where(p => p.EmployeeId == query.EmployeeId.Value);
            }

            if (query.PeriodStart.HasValue)
            {
                filtered = filtered.Where(p => p.PeriodStart >= query.PeriodStart.Value);
            }

            if (query.PeriodEnd.HasValue)
            {
                filtered = filtered.Where(p => p.PeriodEnd <= query.PeriodEnd.Value);
            }

            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;

            var items = filteredList
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new PayrollResponse
                {
                    Id = p.Id,
                    EmployeeId = p.EmployeeId,
                    PeriodStart = p.PeriodStart,
                    PeriodEnd = p.PeriodEnd,
                    GrossAmount = p.GrossAmount,
                    TotalDeductions = p.TotalDeductions,
                    NetAmount = p.NetAmount,
                    Status = p.Status.ToString(),
                    Components = p.Components
                        .Select(c => new PayrollComponentResponse
                        {
                            Type = c.Type.ToString(),
                            Category = c.Category.ToString(),
                            Description = c.Description,
                            Amount = c.Amount
                        })
                        .ToList()
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<PayrollResponse>
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