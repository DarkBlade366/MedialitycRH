using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Rules.Milestones.Commands;
using Application.Features.Payrolls.Rules.Milestones.DTOs;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace Application.Features.Payrolls.Rules.Milestones.Handlers
{
    public class CreateMilestoneRuleHandler
    {
        private readonly IMilestoneRuleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
    
        public CreateMilestoneRuleHandler(
            IMilestoneRuleRepository repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
    
        public async Task<MilestoneRuleResponse> HandleAsync(CreateMilestoneRuleCommand command)
        {
            var rule = new MilestoneRule(
                command.RedmineProjectId,
                command.MilestoneName,
                command.BonusAmount);
    
            await _repository.AddAsync(rule);
            await _unitOfWork.SaveChangesAsync();
    
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