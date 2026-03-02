using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Vacation.DTOs;
using Application.Features.Payrolls.Rules.Vacation.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Vacation.Handlers
{
    public class GetVacationRulesPagedHandler
    {
        private readonly IVacationRuleRepository _repository;
    
        public GetVacationRulesPagedHandler(IVacationRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<VacationRuleResponse>> HandleAsync(GetVacationRulesPagedQuery query)
        {
            var allRules = await _repository.GetAllAsync();
    
            if (query.IsActive.HasValue)
                allRules = allRules.Where(r => r.IsActive == query.IsActive.Value).ToList();
    
            if (query.PayVacationOnUse.HasValue)
                allRules = allRules.Where(r => r.PayVacationOnUse == query.PayVacationOnUse.Value).ToList();
    
            var totalItems = allRules.Count;

            var paged = allRules
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new VacationRuleResponse
                {
                    Id = r.Id,
                    AccrualRatePerMonth = r.AccrualRatePerMonth,
                    PayVacationOnUse = r.PayVacationOnUse,
                    IsActive = r.IsActive
                })
                .ToList();
    
            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);
            return new PagedResponse<VacationRuleResponse>
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