using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Milestones.Handlers
{
    public class GetMilestoneRuleByIdHandler
    {
        private readonly IMilestoneRuleRepository _repository;
    
        public GetMilestoneRuleByIdHandler(IMilestoneRuleRepository repository)
        {
            _repository = repository;
        }
    
        public async Task<MilestoneRuleResponse?> HandleAsync(Guid id)
        {
            var rule = await _repository.GetByIdAsync(id);
    
            if (rule is null)
                return null;
    
            return new MilestoneRuleResponse
            {
                Id = rule.Id,
                RedmineProjectId = rule.RedmineProjectId,
                MilestoneName = rule.MilestoneName,
                BonusAmount = rule.BonusAmount,
                IsActive = rule.IsActive
            };
        }
    }
}