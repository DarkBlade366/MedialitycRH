using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Application.Features.Payrolls.Rules.Milestones.Queries;
using Domain.Features.Payrolls.Interfaces;

namespace Application.Features.Payrolls.Rules.Milestones.Handlers
{
    public class GetMilestoneRulesPagedHandler
    {
        private readonly IMilestoneRuleRepository _repository;

        public GetMilestoneRulesPagedHandler(IMilestoneRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<MilestoneRuleResponse>> HandleAsync(GetMilestoneRulesPagedQuery query)
        {
            var allRules = await _repository.GetAllAsync();

            if (query.isActive.HasValue)
            {
                if (query.isActive.Value)
                {
                    allRules = allRules
                        .Where(x => x.IsActive)
                        .ToList();
                }
                else
                {
                    allRules = allRules
                        .Where(x => !x.IsActive)
                        .ToList();
                }
            }
            
            if (query.ProjectId.HasValue && query.ProjectId.Value > 0)
                allRules = allRules
                    .Where(x => x.RedmineProjectId == query.ProjectId)
                    .ToList();

            var totalItems = allRules.Count;

            var paged = allRules
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