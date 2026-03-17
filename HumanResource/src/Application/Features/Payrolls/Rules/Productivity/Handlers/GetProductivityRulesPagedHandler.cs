using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Productivity.DTOs;
using Application.Features.Payrolls.Rules.Productivity.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Productivity.Handlers
{
    public class GetProductivityRulesPagedHandler
    {
        private readonly IProductivityRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetProductivityRulesPagedHandler(IProductivityRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<ProductivityRuleResponse>> HandleAsync(GetProductivityRulesPagedQuery query)
        {
            string cacheKey = "productivityRules:all";
            var allRules = await _cache.GetAsync<List<ProductivityRule>>(cacheKey);
            if (allRules == null)
            {
                allRules = (await _repository.GetAllAsync())?.ToList() ?? new List<ProductivityRule>();
                await _cache.SetAsync(cacheKey, allRules, TimeSpan.FromMinutes(10));
            }

            var filtered = allRules.AsEnumerable();

            if (query.IsActive.HasValue)
                filtered = filtered.Where(x => x.IsActive == query.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(query.BonusType))
                filtered = filtered.Where(x => x.BonusType.ToString().Equals(query.BonusType, StringComparison.OrdinalIgnoreCase));

            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;

            var paged = filteredList
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