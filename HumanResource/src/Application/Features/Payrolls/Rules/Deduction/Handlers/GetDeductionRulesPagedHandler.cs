using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Deduction.DTOs;
using Application.Features.Payrolls.Rules.Deduction.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Deduction.Handlers
{
    public class GetDeductionRulesPagedHandler
    {
        private readonly IDeductionRuleRepository _repository;
    
        public GetDeductionRulesPagedHandler(IDeductionRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<DeductionRuleResponse>> HandleAsync(GetDeductionRulesPagedQuery query)
        {
            var allRules = await _repository.GetAllAsync();
    
            if (query.IsActive.HasValue)
                allRules = allRules
                    .Where(x => x.IsActive == query.IsActive.Value)
                    .ToList();
    
            if (query.Type.HasValue)
                allRules = allRules
                    .Where(x => x.Type == query.Type.Value)
                    .ToList();
    
            var totalItems = allRules.Count;
    
            var paged = allRules
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new DeductionRuleResponse
                {
                    Id = r.Id,
                    Description = r.Description,
                    Percentage = r.Percentage,
                    Type = r.Type.ToString(),
                    IsActive = r.IsActive
                })
                .ToList();
    
            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);
    
            return new PagedResponse<DeductionRuleResponse>
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