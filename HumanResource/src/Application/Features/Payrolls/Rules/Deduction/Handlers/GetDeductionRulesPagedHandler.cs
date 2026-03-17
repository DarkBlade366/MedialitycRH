using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Deduction.DTOs;
using Application.Features.Payrolls.Rules.Deduction.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Deduction.Handlers
{
    public class GetDeductionRulesPagedHandler
    {
        private readonly IDeductionRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetDeductionRulesPagedHandler(IDeductionRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<DeductionRuleResponse>> HandleAsync(GetDeductionRulesPagedQuery query)
        {
            string cacheKey = "deductionRules:all";
            var allRules = await _cache.GetAsync<List<DeductionRule>>(cacheKey);
            if (allRules == null)
            {
                allRules = (await _repository.GetAllAsync())?.ToList() ?? new List<DeductionRule>();
                await _cache.SetAsync(cacheKey, allRules, TimeSpan.FromMinutes(10));
            }

            var filtered = allRules.AsEnumerable();

            if (query.IsActive.HasValue)
                filtered = filtered.Where(x => x.IsActive == query.IsActive.Value);

            if (query.Type.HasValue)
                filtered = filtered.Where(x => x.Type == query.Type.Value);

            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;

            var paged = filteredList
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