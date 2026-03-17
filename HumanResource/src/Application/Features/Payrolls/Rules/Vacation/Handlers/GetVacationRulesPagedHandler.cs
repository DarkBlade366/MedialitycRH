using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Vacation.DTOs;
using Application.Features.Payrolls.Rules.Vacation.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Vacation.Handlers
{
    public class GetVacationRulesPagedHandler
    {
        private readonly IVacationRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetVacationRulesPagedHandler(IVacationRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<VacationRuleResponse>> HandleAsync(GetVacationRulesPagedQuery query)
        {
            string cacheKey = "vacationRules:all";
            var allRules = await _cache.GetAsync<List<VacationRule>>(cacheKey);
            if (allRules == null)
            {
                allRules = (await _repository.GetAllAsync())?.ToList() ?? new List<VacationRule>();
                await _cache.SetAsync(cacheKey, allRules, TimeSpan.FromMinutes(10));
            }

            var filtered = allRules.AsEnumerable();

            if (query.IsActive.HasValue)
                filtered = filtered.Where(r => r.IsActive == query.IsActive.Value);

            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;

            var paged = filteredList
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new VacationRuleResponse
                {
                    Id = r.Id,
                    AccrualRatePerMonth = r.AccrualRatePerMonth,
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