using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.BaseSalary.DTOs;
using Application.Features.Payrolls.Rules.BaseSalary.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.BaseSalary.Handlers
{
    public class GetBaseSalaryRulesPagedHandler
    {
        private readonly IBaseSalaryRuleRepository _repository;
    
        public GetBaseSalaryRulesPagedHandler(IBaseSalaryRuleRepository repository)
        {
            _repository = repository;
        }
    
        public async Task<PagedResponse<BaseSalaryRuleResponse>> HandleAsync(GetBaseSalaryRulesPagedQuery query)
        {
            var rules = await _repository.GetAllAsync();
    
            if (query.IsActive.HasValue)
                rules = rules.Where(x => x.IsActive == query.IsActive.Value).ToList();
    
            if (query.Role.HasValue)
                rules = rules.Where(x => x.Role == query.Role.Value).ToList();
    
            var totalItems = rules.Count;
    
            var paged = rules
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new BaseSalaryRuleResponse
                {
                    Id = r.Id,
                    Role = r.Role.ToString(),
                    Amount = r.Amount,
                    IsActive = r.IsActive
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);
    
            return new PagedResponse<BaseSalaryRuleResponse>
            {
                Items = paged,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}