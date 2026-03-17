using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Overtime.DTOs;
using Application.Features.Payrolls.Rules.Overtime.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Overtime.Handlers
{
    public class GetOvertimeRulesPagedHandler
    {
        private readonly IOvertimeRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetOvertimeRulesPagedHandler(IOvertimeRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<OvertimeRuleResponse>> HandleAsync(GetOvertimeRulesPagedQuery query)
        {
            string cacheKey = "overtimeRules:all";
            var allRules = await _cache.GetAsync<List<OvertimeRule>>(cacheKey);
            if (allRules == null)
            {
                allRules = (await _repository.GetAllAsync())?.ToList() ?? new List<OvertimeRule>();
                await _cache.SetAsync(cacheKey, allRules, TimeSpan.FromMinutes(10));
            }

            var filtered = allRules.AsEnumerable();

            if (query.IsActive.HasValue)
                filtered = filtered.Where(r => r.IsActive == query.IsActive.Value);

            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;

            var items = filteredList
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new OvertimeRuleResponse
                {
                    Id = r.Id,
                    StandardHoursPerPeriod = r.StandardHoursPerPeriod,
                    OvertimeMultiplier = r.OvertimeMultiplier,
                    HourlyRate = r.HourlyRate,
                    IsActive = r.IsActive
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<OvertimeRuleResponse>
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