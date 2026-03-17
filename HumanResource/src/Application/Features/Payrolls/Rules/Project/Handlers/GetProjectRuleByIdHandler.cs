using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Project.DTOs;
using Application.Features.Payrolls.Rules.Project.Queries;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Project.Handlers
{
    public class GetProjectRuleByIdHandler
    {
        private readonly IProjectRuleRepository _repository;
        private readonly ICacheService _cache;

        public GetProjectRuleByIdHandler(IProjectRuleRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<ProjectRuleResponse?> HandleAsync(GetProjectRuleByIdQuery query)
        {
            string cacheKey = $"projectRule:{query.Id}";
            var cached = await _cache.GetAsync<ProjectRuleResponse>(cacheKey);
            if (cached != null)
                return cached;

            var rule = await _repository.GetByIdAsync(query.Id);
            if (rule == null)
                return null;

            var response = new ProjectRuleResponse
            {
                Id = rule.Id,
                RedmineProjectId = rule.RedmineProjectId,
                BonusAmount = rule.BonusAmount,
                IsActive = rule.IsActive
            };

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            
            return response;
        }
    }
}