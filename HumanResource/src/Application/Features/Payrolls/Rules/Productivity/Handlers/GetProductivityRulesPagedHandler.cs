using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Productivity.DTOs;
using Application.Features.Payrolls.Rules.Productivity.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Productivity.Handlers
{
    public class GetProductivityRulesPagedHandler
    {
        private readonly IProductivityRuleRepository _repository;
    
        public GetProductivityRulesPagedHandler(IProductivityRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<ProductivityRuleResponse>> HandleAsync(GetProductivityRulesPagedQuery query)
        {
            var allRules = await _repository.GetAllAsync();
    
            if (query.IsActive.HasValue)
                allRules = allRules
                    .Where(x => x.IsActive == query.IsActive.Value)
                    .ToList();
    
            if (!string.IsNullOrWhiteSpace(query.BonusType))
                allRules = allRules
                    .Where(x => x.BonusType.ToString().Equals(query.BonusType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
    
            var totalItems = allRules.Count;
    
            var paged = allRules
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new ProductivityRuleResponse
                {
                    Id = r.Id,
                    MinimumTarget = r.MinimumTarget,
                    FullBonusTarget = r.FullBonusTarget,
                    BonusValue = r.BonusValue,
                    BonusType = r.BonusType.ToString(),
                    MaxBonusCap = r.MaxBonusCap,
                    IsActive = r.IsActive
                })
                .ToList();
    
            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);
    
            return new PagedResponse<ProductivityRuleResponse>
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