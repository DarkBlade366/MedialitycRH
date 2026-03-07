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
    
        public GetProjectRuleByIdHandler(IProjectRuleRepository repository)
        {
            _repository = repository;
        }
    
        public async Task<ProjectRuleResponse?> HandleAsync(GetProjectRuleByIdQuery query)
        {
            var rule = await _repository.GetByIdAsync(query.Id);
    
            if (rule is null)
                return null;
    
            return new ProjectRuleResponse
            {
                Id = rule.Id,
                RedmineProjectId = rule.RedmineProjectId,
                BonusAmount = rule.BonusAmount,
                IsActive = rule.IsActive
            };
        }
    }
}
