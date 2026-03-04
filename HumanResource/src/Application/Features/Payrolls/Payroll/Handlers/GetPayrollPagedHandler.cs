using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Payroll.DTOs;
using Application.Features.Payrolls.Payroll.Queries;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Payroll.Handlers
{
    public class GetPayrollPagedHandler
    {
        private readonly IPayrollRepository _repository;
    
        public GetPayrollPagedHandler(IPayrollRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<PayrollResponse>> HandleAsync(GetPayrollPagedQuery query)
        {
            var payrolls = await _repository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                if (Enum.TryParse<PayrollStatus>(query.Status, true, out var parsedStatus))
                {
                    payrolls = payrolls
                        .Where(p => p.Status == parsedStatus)
                        .ToList();
                }
            }

            if (query.EmployeeId.HasValue)
            {
                payrolls = payrolls
                    .Where(p => p.EmployeeId == query.EmployeeId.Value)
                    .ToList();
            }
    
            if (query.PeriodStart.HasValue)
            {
                payrolls = payrolls
                    .Where(p => p.PeriodStart >= query.PeriodStart.Value)
                    .ToList();
            }

            if (query.PeriodEnd.HasValue)
            {
                payrolls = payrolls
                    .Where(p => p.PeriodEnd <= query.PeriodEnd.Value)
                    .ToList();
            }            

            var totalItems = payrolls.Count;
            
            var items = payrolls
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