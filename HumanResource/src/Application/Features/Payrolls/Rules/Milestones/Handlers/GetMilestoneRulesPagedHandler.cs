using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Application.Features.Payrolls.Rules.Milestones.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Milestones.Handlers
{
    public class GetMilestoneRulesPagedHandler
    {
        private readonly IMilestoneRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetMilestoneRulesPagedHandler(IMilestoneRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<PagedResponse<MilestoneRuleResponse>> HandleAsync(GetMilestoneRulesPagedQuery query)
        {
            string cacheKey = "milestoneRules:all";
            var allRules = await _cache.GetAsync<List<MilestoneRule>>(cacheKey);
            if (allRules == null)
            {
                allRules = (await _repository.GetAllAsync())?.ToList() ?? new List<MilestoneRule>();
                await _cache.SetAsync(cacheKey, allRules, TimeSpan.FromMinutes(10));
            }

            var filtered = allRules.AsEnumerable();

            if (query.isActive.HasValue)
                filtered = filtered.Where(x => x.IsActive == query.isActive.Value);

            if (query.ProjectId.HasValue && query.ProjectId.Value > 0)
                filtered = filtered.Where(x => x.RedmineProjectId == query.ProjectId.Value);

            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;

            var paged = filteredList
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new MilestoneRuleResponse
                {
                    Id = r.Id,
                    RedmineProjectId = r.RedmineProjectId,
                    MilestoneName = r.MilestoneName,
                    BonusAmount = r.BonusAmount,
                    IsActive = r.IsActive
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<MilestoneRuleResponse>
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