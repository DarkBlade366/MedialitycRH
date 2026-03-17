using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Project.DTOs;
using Application.Features.Payrolls.Rules.Project.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;
using Application.Common.Interfaces;

namespace Application.Features.Payrolls.Rules.Project.Handlers
{
    public class GetProjectRulesPagedHandler
    {
        private readonly IProjectRuleRepository _repository;
        private readonly ICacheService _cache;
    
        public GetProjectRulesPagedHandler(IProjectRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }
    
        public async Task<PagedResponse<ProjectRuleResponse>> HandleAsync(GetProjectRulesPagedQuery query)
        {
            string cacheKey = "projectRules:all";
            var allRules = await _cache.GetAsync<List<ProjectRule>>(cacheKey);
            if (allRules == null)
            {
                allRules = (await _repository.GetAllAsync())?.ToList() ?? new List<ProjectRule>();
                await _cache.SetAsync(cacheKey, allRules, TimeSpan.FromMinutes(10));
            }

            var filtered = allRules.AsEnumerable();

            if (query.IsActive.HasValue)
                filtered = filtered.Where(x => x.IsActive == query.IsActive.Value);
            if (query.ProjectId.HasValue && query.ProjectId.Value > 0)
                filtered = filtered.Where(x => x.RedmineProjectId == query.ProjectId);

            var filteredList = filtered.ToList();
            var totalItems = filteredList.Count;

            var paged = filteredList
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(r => new ProjectRuleResponse
                {
                    Id = r.Id,
                    RedmineProjectId = r.RedmineProjectId,
                    BonusAmount = r.BonusAmount,
                    IsActive = r.IsActive
                })
                .ToList();

            var totalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PagedResponse<ProjectRuleResponse>
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
