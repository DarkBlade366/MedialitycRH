using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Project.DTOs;
using Application.Features.Payrolls.Rules.Project.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Project.Handlers
{
    public class GetProjectRulesPagedHandler
    {
        private readonly IProjectRuleRepository _repository;
    
        public GetProjectRulesPagedHandler(IProjectRuleRepository repository)
        {
            _repository = repository;
        }
    
        public async Task<PagedResponse<ProjectRuleResponse>> HandleAsync(GetProjectRulesPagedQuery query)
        {
            var allRules = await _repository.GetAllAsync();
    
            if (query.IsActive.HasValue)
                allRules = allRules
                    .Where(x => x.IsActive == query.IsActive.Value)
                    .ToList();

            if (query.ProjectId.HasValue && query.ProjectId.Value > 0)
                allRules = allRules
                    .Where(x => x.RedmineProjectId == query.ProjectId)
                    .ToList();
    
            var totalItems = allRules.Count;
    
            var paged = allRules
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
